'use client';

import { useState } from 'react';
import Navbar from '@/components/Navbar';
import Footer from '@/components/Footer';
import api from '@/lib/api';
import toast from 'react-hot-toast';
import { FiMail, FiPhone, FiMapPin, FiSend } from 'react-icons/fi';

export default function ContactPage() {
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    phoneNumber: '',
    message: '',
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      await api.post('/admincontact', formData);
      toast.success('تم إرسال رسالتك بنجاح');
      setFormData({ name: '', email: '', phoneNumber: '', message: '' });
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'حدث خطأ');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />

      <div className="max-w-7xl mx-auto px-4 py-8">
        <h1 className="text-3xl font-bold text-center mb-8">تواصل معنا</h1>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          {/* معلومات التواصل */}
          <div className="space-y-6">
            <div className="bg-white rounded-lg shadow-md p-6">
              <h2 className="text-2xl font-bold mb-6">معلومات الاتصال</h2>
              
              <div className="space-y-4">
                <div className="flex items-start gap-4">
                  <div className="bg-primary-100 p-3 rounded-lg">
                    <FiMapPin className="text-primary-600" size={24} />
                  </div>
                  <div>
                    <h3 className="font-semibold mb-1">العنوان</h3>
                    <p className="text-gray-600">القاهرة، مصر</p>
                  </div>
                </div>

                <div className="flex items-start gap-4">
                  <div className="bg-primary-100 p-3 rounded-lg">
                    <FiPhone className="text-primary-600" size={24} />
                  </div>
                  <div>
                    <h3 className="font-semibold mb-1">الهاتف</h3>
                    <p className="text-gray-600">+20 123 456 7890</p>
                  </div>
                </div>

                <div className="flex items-start gap-4">
                  <div className="bg-primary-100 p-3 rounded-lg">
                    <FiMail className="text-primary-600" size={24} />
                  </div>
                  <div>
                    <h3 className="font-semibold mb-1">البريد الإلكتروني</h3>
                    <p className="text-gray-600">info@electro.com</p>
                  </div>
                </div>
              </div>
            </div>

            <div className="bg-primary-600 text-white rounded-lg shadow-md p-6">
              <h2 className="text-2xl font-bold mb-4">ساعات العمل</h2>
              <div className="space-y-2">
                <p>السبت - الخميس: 9:00 ص - 10:00 م</p>
                <p>الجمعة: 2:00 م - 10:00 م</p>
              </div>
            </div>
          </div>

          {/* نموذج التواصل */}
          <div className="bg-white rounded-lg shadow-md p-6">
            <h2 className="text-2xl font-bold mb-6">أرسل رسالة</h2>

            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  الاسم
                </label>
                <input
                  type="text"
                  required
                  value={formData.name}
                  onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  placeholder="أدخل اسمك"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  البريد الإلكتروني
                </label>
                <input
                  type="email"
                  required
                  value={formData.email}
                  onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  placeholder="أدخل بريدك الإلكتروني"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  رقم الهاتف
                </label>
                <input
                  type="tel"
                  required
                  value={formData.phoneNumber}
                  onChange={(e) => setFormData({ ...formData, phoneNumber: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  placeholder="أدخل رقم هاتفك"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  الرسالة
                </label>
                <textarea
                  required
                  value={formData.message}
                  onChange={(e) => setFormData({ ...formData, message: e.target.value })}
                  rows={5}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  placeholder="اكتب رسالتك هنا"
                ></textarea>
              </div>

              <button
                type="submit"
                disabled={loading}
                className="w-full bg-primary-600 text-white py-3 rounded-lg hover:bg-primary-700 flex items-center justify-center gap-2 disabled:bg-gray-400"
              >
                <FiSend />
                <span>{loading ? 'جاري الإرسال...' : 'إرسال الرسالة'}</span>
              </button>
            </form>
          </div>
        </div>
      </div>

      <Footer />
    </div>
  );
}

