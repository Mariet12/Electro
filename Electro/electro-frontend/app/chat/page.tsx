'use client';

import { useState, useEffect, useRef } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { useRouter } from 'next/navigation';
import Navbar from '@/components/Navbar';
import Footer from '@/components/Footer';
import api from '@/lib/api';
import toast from 'react-hot-toast';
import { FiSend, FiMessageCircle, FiSearch, FiArrowLeft } from 'react-icons/fi';
import { format } from 'date-fns';
import { ar } from 'date-fns/locale';
// SignalR will be loaded dynamically to avoid build-time errors
// @ts-ignore - SignalR package will be installed at runtime
let signalR: any = null;

interface Conversation {
  id: number;
  senderId: string;
  receiverId: string;
  userName: string;
  userImage: string;
  lastMessageContent?: string;
  lastMessageTimestamp?: string;
  unreadMessagesCount?: number;
}

interface Message {
  id?: number;
  conversationId: number;
  senderId: string;
  receiverId: string;
  content: string;
  sentAt: string;
  isRead: boolean;
  senderName?: string;
  senderImage?: string;
}

export default function ChatPage() {
  const { user } = useAuth();
  const router = useRouter();
  const [conversations, setConversations] = useState<Conversation[]>([]);
  const [selectedConversation, setSelectedConversation] = useState<Conversation | null>(null);
  const [messages, setMessages] = useState<Message[]>([]);
  const [messageText, setMessageText] = useState('');
  const [loading, setLoading] = useState(true);
  const [sending, setSending] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const connectionRef = useRef<any>(null);

  useEffect(() => {
    if (!user) {
      router.push('/login');
      return;
    }
    fetchConversations();
    initializeSignalR();
    return () => {
      if (connectionRef.current) {
        connectionRef.current.stop();
      }
    };
  }, [user, router]);

  useEffect(() => {
    if (selectedConversation) {
      fetchMessages(selectedConversation.id);
    }
  }, [selectedConversation]);

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  const initializeSignalR = async () => {
    if (!user?.id) return;

    try {
      const token = localStorage.getItem('token');
      if (!token) return;

      // Dynamic import for SignalR
      if (!signalR) {
        try {
          // @ts-ignore
          signalR = await import('@microsoft/signalr');
        } catch (err) {
          console.warn('SignalR package not installed. Real-time features will be limited.');
          return;
        }
      }

      // الحصول على URL الـbackend الصحيح
      let hubUrl: string;
      if (typeof window !== 'undefined') {
        if (window.location.hostname === 'localhost') {
          // Development: استخدام localhost مباشرة
          hubUrl = 'http://localhost:5008/ChatHub';
        } else {
          // Production: استخدام الـbackend URL مباشرة (ليس عبر proxy)
          const backendUrl = process.env.NEXT_PUBLIC_API_URL || 'https://elctroapp.runasp.net/api';
          hubUrl = backendUrl.replace('/api', '') + '/ChatHub';
        }
      } else {
        hubUrl = 'https://elctroapp.runasp.net/ChatHub';
      }
      
      const HttpTransportType = signalR.HttpTransportType || { WebSockets: 0, LongPolling: 1 };
      const LogLevel = signalR.LogLevel || { Information: 2 };
      
      const connection = new signalR.HubConnectionBuilder()
        .withUrl(hubUrl, {
          accessTokenFactory: () => token,
          skipNegotiation: false,
          transport: (HttpTransportType.WebSockets || 0) | (HttpTransportType.LongPolling || 1),
        } as any)
        .withAutomaticReconnect({
          nextRetryDelayInMilliseconds: (retryContext: any) => {
            if (retryContext.previousRetryCount < 3) {
              return 2000; // 2 ثواني
            }
            return 5000; // 5 ثواني بعد المحاولة الثالثة
          }
        })
        .configureLogging(LogLevel.Information || 2)
        .build();

      connection.on('ReceiveMessages', (conversationId: number, senderId: string, senderName: string, senderImage: string, receiverId: string, content: string, date: string) => {
        if (selectedConversation?.id === conversationId) {
          setMessages((prev) => [
            ...prev,
            {
              conversationId,
              senderId,
              receiverId,
              content,
              sentAt: date,
              isRead: receiverId === user.id,
              senderName,
              senderImage,
            },
          ]);
        }
        fetchConversations(); // تحديث قائمة المحادثات
      });

      connection.on('ReceiveMessage', (data: any) => {
        if (selectedConversation?.id === data.conversationId) {
          setMessages((prev) => [
            ...prev,
            {
              conversationId: data.conversationId,
              senderId: data.messageDetails.sender.senderId,
              receiverId: data.messageDetails.recipient.recipientId,
              content: data.messageDetails.content,
              sentAt: data.messageDetails.sentAt,
              isRead: false,
              senderName: data.messageDetails.sender.senderName,
              senderImage: data.messageDetails.sender.senderImage,
            },
          ]);
        }
        fetchConversations();
      });

      await connection.start();
      connectionRef.current = connection;
      console.log('✅ SignalR connected successfully');
      
      // إضافة معالجات للأحداث
      connection.onclose((error: any) => {
        console.log('SignalR connection closed', error);
      });
      
      connection.onreconnecting((error: any) => {
        console.log('SignalR reconnecting...', error);
      });
      
      connection.onreconnected((connectionId: string) => {
        console.log('SignalR reconnected', connectionId);
      });
    } catch (error) {
      console.error('SignalR connection error:', error);
      toast.error('فشل الاتصال بالشات المباشر. سيتم تحديث الرسائل يدوياً.');
    }
  };

  const fetchConversations = async () => {
    try {
      const response = await api.get('/chat/all-chats');
      const data = response.data?.data || [];
      setConversations(Array.isArray(data) ? data : []);
    } catch (error) {
      console.error('Error fetching conversations:', error);
    } finally {
      setLoading(false);
    }
  };

  const fetchMessages = async (conversationId: number) => {
    try {
      const response = await api.get(`/chat/get-messages/${conversationId}`);
      const data = response.data?.data || [];
      setMessages(Array.isArray(data) ? data : []);
      
      // وضع علامة على الرسائل كمقروءة
      if (data.length > 0) {
        await api.post('/chat/mark-all-messages-as-read', {
          ConversationId: conversationId,
          UserId: user?.id,
        });
      }
    } catch (error) {
      console.error('Error fetching messages:', error);
    }
  };

  const sendMessage = async () => {
    if (!messageText.trim() || !selectedConversation || !user?.id) return;

    setSending(true);
    try {
      const receiverId = selectedConversation.senderId === user.id 
        ? selectedConversation.receiverId 
        : selectedConversation.senderId;

      await api.post('/chat/send-message', {
        ConversationId: selectedConversation.id,
        SenderId: user.id,
        ReceiverId: receiverId,
        Content: messageText.trim(),
      });

      setMessageText('');
      
      // إضافة الرسالة فوراً للواجهة
      const newMessage: Message = {
        conversationId: selectedConversation.id,
        senderId: user.id,
        receiverId: receiverId,
        content: messageText.trim(),
        sentAt: new Date().toISOString(),
        isRead: false,
        senderName: user.displayName || user.email,
        senderImage: user.imageUrl,
      };
      setMessages((prev) => [...prev, newMessage]);
      fetchConversations();
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'فشل إرسال الرسالة');
    } finally {
      setSending(false);
    }
  };

  const createConversationWithAdmin = async () => {
    if (!user?.id) return;
    
    try {
      // محاولة الحصول على قائمة المستخدمين مع فلترة الأدمن
      let adminId: string | null = null;
      
      try {
        const usersResponse = await api.get('/account/users-by-status');
        const users = usersResponse.data?.data || [];
        const admin = users.find((u: any) => u.role?.toLowerCase() === 'admin' || u.role?.toLowerCase() === 'administrator');
        if (admin) {
          adminId = admin.id;
        }
      } catch (e) {
        console.log('Could not fetch users, trying alternative method');
      }

      // إذا لم نجد أدمن، نستخدم معرف افتراضي أو نطلب من المستخدم إدخاله
      if (!adminId) {
        toast.error('لا يمكن العثور على أدمن. يرجى التواصل مع الدعم الفني.');
        return;
      }
      
      const response = await api.post('/chat/create-conversation', {
        SenderId: user.id,
        ReceiverId: adminId,
      });

      const conversation = response.data?.data;
      if (conversation) {
        setSelectedConversation({
          id: conversation.Id,
          senderId: conversation.SenderId,
          receiverId: conversation.ReceiverId,
          userName: conversation.UserName,
          userImage: conversation.UserImage,
        });
        fetchConversations(); // تحديث القائمة
      }
    } catch (error: any) {
      console.error('Error creating conversation:', error);
      toast.error(error.response?.data?.message || 'فشل إنشاء المحادثة');
    }
  };

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  const filteredConversations = conversations.filter((conv) =>
    conv.userName?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50">
        <Navbar />
        <div className="max-w-7xl mx-auto px-4 py-8">
          <div className="text-center">جاري التحميل...</div>
        </div>
        <Footer />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />

      <div className="max-w-7xl mx-auto px-4 py-8">
        <div className="bg-white rounded-lg shadow-md overflow-hidden" style={{ height: '600px' }}>
          <div className="flex h-full">
            {/* قائمة المحادثات */}
            <div className="w-1/3 border-l border-gray-200 flex flex-col">
              <div className="p-4 border-b border-gray-200">
                <div className="flex items-center gap-2 mb-4">
                  <button
                    onClick={() => router.back()}
                    className="p-2 hover:bg-gray-100 rounded-lg"
                  >
                    <FiArrowLeft />
                  </button>
                  <h2 className="text-xl font-bold">المحادثات</h2>
                </div>
                <div className="relative">
                  <FiSearch className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                  <input
                    type="text"
                    placeholder="بحث..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="w-full pr-10 pl-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-primary-500"
                  />
                </div>
              </div>

              <div className="flex-1 overflow-y-auto">
                {filteredConversations.length === 0 ? (
                  <div className="p-4 text-center text-gray-500">
                    {searchTerm ? 'لا توجد نتائج' : 'لا توجد محادثات'}
                    {!searchTerm && (
                      <button
                        onClick={createConversationWithAdmin}
                        className="mt-4 block w-full text-primary-600 hover:text-primary-700"
                      >
                        بدء محادثة مع الأدمن
                      </button>
                    )}
                  </div>
                ) : (
                  filteredConversations.map((conv) => (
                    <div
                      key={conv.id}
                      onClick={() => setSelectedConversation(conv)}
                      className={`p-4 border-b border-gray-200 cursor-pointer hover:bg-gray-50 ${
                        selectedConversation?.id === conv.id ? 'bg-primary-50' : ''
                      }`}
                    >
                      <div className="flex items-center gap-3">
                        <img
                          src={conv.userImage || '/placeholder.png'}
                          alt={conv.userName}
                          className="w-12 h-12 rounded-full object-cover"
                        />
                        <div className="flex-1 min-w-0">
                          <h3 className="font-semibold truncate">{conv.userName}</h3>
                          <p className="text-sm text-gray-600 truncate">
                            {conv.lastMessageContent || 'لا توجد رسائل'}
                          </p>
                          {conv.lastMessageTimestamp && (
                            <p className="text-xs text-gray-400 mt-1">
                              {format(new Date(conv.lastMessageTimestamp), 'PPp', { locale: ar })}
                            </p>
                          )}
                        </div>
                        {conv.unreadMessagesCount && conv.unreadMessagesCount > 0 && (
                          <span className="bg-primary-600 text-white text-xs rounded-full px-2 py-1">
                            {conv.unreadMessagesCount}
                          </span>
                        )}
                      </div>
                    </div>
                  ))
                )}
              </div>
            </div>

            {/* منطقة الرسائل */}
            <div className="flex-1 flex flex-col">
              {selectedConversation ? (
                <>
                  <div className="p-4 border-b border-gray-200 bg-gray-50">
                    <div className="flex items-center gap-3">
                      <img
                        src={selectedConversation.userImage || '/placeholder.png'}
                        alt={selectedConversation.userName}
                        className="w-10 h-10 rounded-full object-cover"
                      />
                      <div>
                        <h3 className="font-semibold">{selectedConversation.userName}</h3>
                      </div>
                    </div>
                  </div>

                  <div className="flex-1 overflow-y-auto p-4 space-y-4">
                    {messages.map((msg, idx) => {
                      const isOwn = msg.senderId === user?.id;
                      return (
                        <div
                          key={idx}
                          className={`flex ${isOwn ? 'justify-end' : 'justify-start'}`}
                        >
                          <div
                            className={`max-w-xs lg:max-w-md px-4 py-2 rounded-lg ${
                              isOwn
                                ? 'bg-primary-600 text-white'
                                : 'bg-gray-200 text-gray-800'
                            }`}
                          >
                            <p>{msg.content}</p>
                            <p
                              className={`text-xs mt-1 ${
                                isOwn ? 'text-primary-100' : 'text-gray-500'
                              }`}
                            >
                              {format(new Date(msg.sentAt), 'p', { locale: ar })}
                            </p>
                          </div>
                        </div>
                      );
                    })}
                    <div ref={messagesEndRef} />
                  </div>

                  <div className="p-4 border-t border-gray-200">
                    <div className="flex gap-2">
                      <input
                        type="text"
                        value={messageText}
                        onChange={(e) => setMessageText(e.target.value)}
                        onKeyPress={(e) => e.key === 'Enter' && !e.shiftKey && sendMessage()}
                        placeholder="اكتب رسالة..."
                        className="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-primary-500"
                      />
                      <button
                        onClick={sendMessage}
                        disabled={sending || !messageText.trim()}
                        className="bg-primary-600 text-white px-6 py-2 rounded-lg hover:bg-primary-700 disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-2"
                      >
                        <FiSend />
                        {sending ? 'جاري الإرسال...' : 'إرسال'}
                      </button>
                    </div>
                  </div>
                </>
              ) : (
                <div className="flex-1 flex items-center justify-center text-gray-500">
                  <div className="text-center">
                    <FiMessageCircle size={64} className="mx-auto mb-4 text-gray-400" />
                    <p>اختر محادثة للبدء</p>
                    <button
                      onClick={createConversationWithAdmin}
                      className="mt-4 text-primary-600 hover:text-primary-700"
                    >
                      أو ابدأ محادثة جديدة مع الأدمن
                    </button>
                  </div>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>

      <Footer />
    </div>
  );
}

