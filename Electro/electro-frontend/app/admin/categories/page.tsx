'use client';

import { useState, useEffect } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { useRouter } from 'next/navigation';
import Navbar from '@/components/Navbar';
import Footer from '@/components/Footer';
import api from '@/lib/api';
import toast from 'react-hot-toast';
import { FiPlus, FiEdit2, FiTrash2 } from 'react-icons/fi';
import Link from 'next/link';

export default function AdminCategoriesPage() {
  const { user } = useAuth();
  const router = useRouter();
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showAddModal, setShowAddModal] = useState(false);

  useEffect(() => {
    if (!user) {
      return;
    }
    
    const isAdmin = user.roles?.includes('Admin') || user.role === 'Admin' || user.roles?.some((r: string) => r.toLowerCase() === 'admin');
    
    if (!isAdmin) {
      router.push('/');
      return;
    }
    
    fetchCategories();
  }, [user, router]);

  const fetchCategories = async () => {
    try {
      const response = await api.get('/category?pageSize=100');
      setCategories(response.data.data.items || []);
    } catch (error) {
      console.error('Error fetching categories:', error);
    } finally {
      setLoading(false);
    }
  };

  const deleteCategory = async (id: number) => {
    if (!confirm('هل أنت متأكد من حذف هذه الفئة؟')) return;

    try {
      await api.delete(`/category/${id}`);
      toast.success('تم حذف الفئة بنجاح');
      fetchCategories();
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'حدث خطأ');
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />

      <div className="max-w-6xl mx-auto px-4 py-8">
        <div className="flex justify-between items-center mb-8">
          <h1 className="text-3xl font-bold text-gray-800">إدارة الفئات</h1>
          <div className="flex gap-4">
            <Link href="/admin" className="text-blue-600 hover:text-blue-700">
              ← العودة للوحة التحكم
            </Link>
            <button
              onClick={() => setShowAddModal(true)}
              className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 flex items-center gap-2"
            >
              <FiPlus />
              <span>إضافة فئة جديدة</span>
            </button>
          </div>
        </div>

        {loading ? (
          <div className="text-center py-12">
            <p className="text-gray-600">جاري التحميل...</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {categories.length === 0 ? (
              <div className="col-span-full text-center py-12 bg-white rounded-lg">
                <p className="text-gray-600">لا توجد فئات. اضغط &quot;إضافة فئة جديدة&quot; للبدء!</p>
              </div>
            ) : (
              categories.map((category: any) => (
                <div key={category.id} className="bg-white rounded-lg shadow-md p-6">
                  {category.imageUrl && (
                    <img
                      src={category.imageUrl}
                      alt={category.name}
                      className="w-full h-40 object-cover rounded-lg mb-4"
                    />
                  )}
                  <h3 className="text-lg font-bold text-gray-800 mb-2">{category.name}</h3>
                  {category.description && (
                    <p className="text-sm text-gray-600 mb-4">{category.description}</p>
                  )}
                  <div className="flex gap-2">
                    <button
                      onClick={() => deleteCategory(category.id)}
                      className="flex-1 text-red-600 border border-red-600 px-4 py-2 rounded-lg hover:bg-red-50 flex items-center justify-center gap-2"
                    >
                      <FiTrash2 size={16} />
                      <span>حذف</span>
                    </button>
                  </div>
                </div>
              ))
            )}
          </div>
        )}

        {showAddModal && (
          <AddCategoryModal
            onClose={() => setShowAddModal(false)}
            onSuccess={() => {
              setShowAddModal(false);
              fetchCategories();
            }}
          />
        )}
      </div>

      <Footer />
    </div>
  );
}

function AddCategoryModal({
  onClose,
  onSuccess,
}: {
  onClose: () => void;
  onSuccess: () => void;
}) {
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState({
    name: '',
    description: '',
  });
  const [image, setImage] = useState<File | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      const data = new FormData();
      data.append('name', formData.name);
      data.append('description', formData.description);
      if (image) {
        data.append('image', image);
      }

      await api.post('/category', data, {
        headers: { 'Content-Type': 'multipart/form-data' },
      });

      toast.success('تم إضافة الفئة بنجاح');
      onSuccess();
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'حدث خطأ');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-lg max-w-md w-full p-6">
        <h2 className="text-2xl font-bold mb-6 text-gray-800">إضافة فئة جديدة</h2>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              اسم الفئة *
            </label>
            <input
              type="text"
              required
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900"
              placeholder="أجهزة طبية"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              الوصف
            </label>
            <textarea
              value={formData.description}
              onChange={(e) => setFormData({ ...formData, description: e.target.value })}
              rows={3}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900"
              placeholder="وصف الفئة..."
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              صورة الفئة
            </label>
            <input
              type="file"
              accept="image/*"
              onChange={(e) => setImage(e.target.files?.[0] || null)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900"
            />
          </div>

          <div className="flex gap-4 pt-4">
            <button
              type="submit"
              disabled={loading}
              className="flex-1 bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 disabled:bg-gray-400"
            >
              {loading ? 'جاري الإضافة...' : 'إضافة'}
            </button>
            <button
              type="button"
              onClick={onClose}
              className="flex-1 bg-gray-200 text-gray-800 py-2 rounded-lg hover:bg-gray-300"
            >
              إلغاء
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

