'use client';

import { useState, useEffect } from 'react';
import Navbar from '@/components/Navbar';
import Footer from '@/components/Footer';
import api from '@/lib/api';
import { FiBell, FiCheckCircle } from 'react-icons/fi';
import { format } from 'date-fns';
import { ar } from 'date-fns/locale';

export default function NotificationsPage() {
  const [notifications, setNotifications] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchNotifications();
  }, []);

  const fetchNotifications = async () => {
    try {
      const response = await api.get('/notifications');
      setNotifications(response.data.data || []);
    } catch (error) {
      console.error('Error fetching notifications:', error);
    } finally {
      setLoading(false);
    }
  };

  const markAsRead = async (id: number) => {
    try {
      await api.put(`/notifications/${id}/read`);
      fetchNotifications();
    } catch (error) {
      console.error('Error marking notification as read:', error);
    }
  };

  const markAllAsRead = async () => {
    try {
      await api.put('/notifications/mark-all-read');
      fetchNotifications();
    } catch (error) {
      console.error('Error marking all notifications as read:', error);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />

      <div className="max-w-4xl mx-auto px-4 py-8">
        <div className="flex justify-between items-center mb-8">
          <h1 className="text-3xl font-bold">الإشعارات</h1>
          {notifications.some((n: any) => !n.isRead) && (
            <button
              onClick={markAllAsRead}
              className="text-primary-600 hover:text-primary-700"
            >
              تعليم الكل كمقروء
            </button>
          )}
        </div>

        {loading ? (
          <div className="space-y-4">
            {[...Array(3)].map((_, i) => (
              <div key={i} className="bg-white rounded-lg shadow-md h-24 animate-pulse"></div>
            ))}
          </div>
        ) : notifications.length === 0 ? (
          <div className="bg-white rounded-lg shadow-md p-12 text-center">
            <FiBell size={64} className="mx-auto text-gray-400 mb-4" />
            <h2 className="text-2xl font-bold text-gray-800 mb-2">لا توجد إشعارات</h2>
            <p className="text-gray-600">ليس لديك أي إشعارات حالياً</p>
          </div>
        ) : (
          <div className="space-y-4">
            {notifications.map((notification: any) => (
              <div
                key={notification.id}
                className={`bg-white rounded-lg shadow-md p-6 ${
                  !notification.isRead ? 'border-r-4 border-primary-600' : ''
                }`}
              >
                <div className="flex justify-between items-start">
                  <div className="flex-1">
                    <h3 className="font-semibold text-lg mb-2">{notification.title}</h3>
                    <p className="text-gray-600 mb-3">{notification.message}</p>
                    <p className="text-sm text-gray-500">
                      {notification.createdAt &&
                        format(new Date(notification.createdAt), 'PPp', { locale: ar })}
                    </p>
                  </div>
                  {!notification.isRead && (
                    <button
                      onClick={() => markAsRead(notification.id)}
                      className="text-primary-600 hover:text-primary-700 ml-4"
                    >
                      <FiCheckCircle size={20} />
                    </button>
                  )}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      <Footer />
    </div>
  );
}

