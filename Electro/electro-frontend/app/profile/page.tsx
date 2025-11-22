'use client';

import { useState, useEffect } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import Navbar from '@/components/Navbar';
import Footer from '@/components/Footer';
import api from '@/lib/api';
import toast from 'react-hot-toast';
import { FiUser, FiMail, FiPhone, FiCamera, FiLock } from 'react-icons/fi';
import Image from 'next/image';

export default function ProfilePage() {
  const { user, updateUser } = useAuth();
  const [isEditing, setIsEditing] = useState(false);
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState({
    displayName: '',
    phoneNumber: '',
    image: null as File | null,
  });
  const [changePassword, setChangePassword] = useState({
    currentPassword: '',
    newPassword: '',
    confirmPassword: '',
  });

  useEffect(() => {
    if (user) {
      setFormData({
        displayName: user.displayName || '',
        phoneNumber: user.phoneNumber || '',
        image: null,
      });
    }
  }, [user]);

  const handleUpdateProfile = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      const formDataToSend = new FormData();
      formDataToSend.append('displayName', formData.displayName);
      formDataToSend.append('phoneNumber', formData.phoneNumber);
      if (formData.image) {
        formDataToSend.append('image', formData.image);
      }

      await api.put('/account/update-user', formDataToSend, {
        headers: { 'Content-Type': 'multipart/form-data' },
      });
      await updateUser();
      toast.success('تم تحديث الملف الشخصي');
      setIsEditing(false);
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'حدث خطأ');
    } finally {
      setLoading(false);
    }
  };

  const handleChangePassword = async (e: React.FormEvent) => {
    e.preventDefault();

    if (changePassword.newPassword !== changePassword.confirmPassword) {
      toast.error('كلمة المرور غير متطابقة');
      return;
    }

    setLoading(true);
    try {
      await api.put('/account/change-password', {
        currentPassword: changePassword.currentPassword,
        newPassword: changePassword.newPassword,
      });
      toast.success('تم تغيير كلمة المرور');
      setChangePassword({ currentPassword: '', newPassword: '', confirmPassword: '' });
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'كلمة المرور الحالية غير صحيحة');
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteImage = async () => {
    try {
      await api.delete('/account/delete-image');
      await updateUser();
      toast.success('تم حذف الصورة');
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'حدث خطأ');
    }
  };

  if (!user) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <p className="text-gray-600">جاري التحميل...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />
      
      <div className="max-w-4xl mx-auto px-4 py-8">
        <h1 className="text-3xl font-bold mb-8">الملف الشخصي</h1>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          {/* الصورة الشخصية */}
          <div className="md:col-span-1">
            <div className="bg-white rounded-lg shadow-md p-6 text-center">
              <div className="relative w-32 h-32 mx-auto mb-4">
                {user.imageUrl ? (
                  <Image
                    src={user.imageUrl}
                    alt={user.displayName}
                    fill
                    className="rounded-full object-cover"
                  />
                ) : (
                  <div className="w-32 h-32 bg-gray-200 rounded-full flex items-center justify-center">
                    <FiUser size={48} className="text-gray-400" />
                  </div>
                )}
                {isEditing && (
                  <label className="absolute bottom-0 right-0 bg-primary-600 p-2 rounded-full cursor-pointer hover:bg-primary-700">
                    <FiCamera className="text-white" />
                    <input
                      type="file"
                      accept="image/*"
                      className="hidden"
                      onChange={(e) => {
                        if (e.target.files?.[0]) {
                          setFormData({ ...formData, image: e.target.files[0] });
                        }
                      }}
                    />
                  </label>
                )}
              </div>
              <h2 className="text-xl font-semibold">{user.displayName}</h2>
              <p className="text-gray-600">{user.email}</p>
              {user.imageUrl && (
                <button
                  onClick={handleDeleteImage}
                  className="mt-4 text-red-600 text-sm hover:underline"
                >
                  حذف الصورة
                </button>
              )}
            </div>
          </div>

          {/* معلومات الحساب */}
          <div className="md:col-span-2 space-y-6">
            {/* تعديل المعلومات */}
            <div className="bg-white rounded-lg shadow-md p-6">
              <div className="flex justify-between items-center mb-6">
                <h3 className="text-xl font-semibold">المعلومات الشخصية</h3>
                <button
                  onClick={() => setIsEditing(!isEditing)}
                  className="text-primary-600 hover:text-primary-700"
                >
                  {isEditing ? 'إلغاء' : 'تعديل'}
                </button>
              </div>

              <form onSubmit={handleUpdateProfile} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    الاسم
                  </label>
                  <div className="relative">
                    <FiUser className="absolute right-3 top-3 text-gray-400" />
                    <input
                      type="text"
                      value={formData.displayName}
                      onChange={(e) => setFormData({ ...formData, displayName: e.target.value })}
                      disabled={!isEditing}
                      className="w-full pr-10 px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-primary-500 focus:border-primary-500 disabled:bg-gray-100"
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    البريد الإلكتروني
                  </label>
                  <div className="relative">
                    <FiMail className="absolute right-3 top-3 text-gray-400" />
                    <input
                      type="email"
                      value={user.email}
                      disabled
                      className="w-full pr-10 px-3 py-2 border border-gray-300 rounded-lg bg-gray-100"
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    رقم الهاتف
                  </label>
                  <div className="relative">
                    <FiPhone className="absolute right-3 top-3 text-gray-400" />
                    <input
                      type="tel"
                      value={formData.phoneNumber}
                      onChange={(e) => setFormData({ ...formData, phoneNumber: e.target.value })}
                      disabled={!isEditing}
                      className="w-full pr-10 px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-primary-500 focus:border-primary-500 disabled:bg-gray-100"
                    />
                  </div>
                </div>

                {isEditing && (
                  <button
                    type="submit"
                    disabled={loading}
                    className="w-full bg-primary-600 text-white py-2 rounded-lg hover:bg-primary-700 disabled:bg-gray-400"
                  >
                    {loading ? 'جاري الحفظ...' : 'حفظ التغييرات'}
                  </button>
                )}
              </form>
            </div>

            {/* تغيير كلمة المرور */}
            <div className="bg-white rounded-lg shadow-md p-6">
              <h3 className="text-xl font-semibold mb-6">تغيير كلمة المرور</h3>
              
              <form onSubmit={handleChangePassword} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    كلمة المرور الحالية
                  </label>
                  <div className="relative">
                    <FiLock className="absolute right-3 top-3 text-gray-400" />
                    <input
                      type="password"
                      value={changePassword.currentPassword}
                      onChange={(e) => setChangePassword({ ...changePassword, currentPassword: e.target.value })}
                      className="w-full pr-10 px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    كلمة المرور الجديدة
                  </label>
                  <div className="relative">
                    <FiLock className="absolute right-3 top-3 text-gray-400" />
                    <input
                      type="password"
                      value={changePassword.newPassword}
                      onChange={(e) => setChangePassword({ ...changePassword, newPassword: e.target.value })}
                      className="w-full pr-10 px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    تأكيد كلمة المرور الجديدة
                  </label>
                  <div className="relative">
                    <FiLock className="absolute right-3 top-3 text-gray-400" />
                    <input
                      type="password"
                      value={changePassword.confirmPassword}
                      onChange={(e) => setChangePassword({ ...changePassword, confirmPassword: e.target.value })}
                      className="w-full pr-10 px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                </div>

                <button
                  type="submit"
                  disabled={loading}
                  className="w-full bg-primary-600 text-white py-2 rounded-lg hover:bg-primary-700 disabled:bg-gray-400"
                >
                  {loading ? 'جاري الحفظ...' : 'تغيير كلمة المرور'}
                </button>
              </form>
            </div>
          </div>
        </div>
      </div>

      <Footer />
    </div>
  );
}

