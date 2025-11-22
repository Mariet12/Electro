'use client';

import { useState, useEffect } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { useRouter, useParams } from 'next/navigation';
import Navbar from '@/components/Navbar';
import Footer from '@/components/Footer';
import api from '@/lib/api';
import toast from 'react-hot-toast';
import { FiArrowRight } from 'react-icons/fi';
import Link from 'next/link';
import Image from 'next/image';

export default function EditProductPage() {
  const { user } = useAuth();
  const router = useRouter();
  const params = useParams();
  const productId = params?.id as string;

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [categories, setCategories] = useState([]);
  const [formData, setFormData] = useState({
    name_Ar: '',
    name_En: '',
    description: '',
    price: '',
    discountedPrice: '',
    stock: '',
    categoryId: '',
    brand: '',
    countryOfOrigin: '',
    warranty: '',
  });
  const [images, setImages] = useState<FileList | null>(null);
  const [existingImages, setExistingImages] = useState<any[]>([]);
  const [imagesToDelete, setImagesToDelete] = useState<number[]>([]);

  useEffect(() => {
    if (!user) {
      return;
    }
    
    const isAdmin = user.roles?.includes('Admin') || user.role === 'Admin' || user.roles?.some((r: string) => r?.toLowerCase() === 'admin');
    
    if (!isAdmin) {
      router.push('/');
      return;
    }
    
    fetchProduct();
    fetchCategories();
  }, [user, router, productId]);

  const fetchProduct = async () => {
    try {
      const response = await api.get(`/products/${productId}`);
      const product = response.data.data;
      
      setFormData({
        name_Ar: product.name_Ar || '',
        name_En: product.name_En || '',
        description: product.description || '',
        price: product.price?.toString() || '',
        discountedPrice: product.discountedPrice?.toString() || '',
        stock: product.stock?.toString() || '',
        categoryId: product.categoryId?.toString() || '',
        brand: product.brand || '',
        countryOfOrigin: product.countryOfOrigin || '',
        warranty: product.warranty || '',
      });
      
      setExistingImages(product.images || []);
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'فشل تحميل المنتج');
      router.push('/admin/products');
    } finally {
      setLoading(false);
    }
  };

  const fetchCategories = async () => {
    try {
      const response = await api.get('/category?pageSize=100');
      setCategories(response.data.data.items || []);
    } catch (error) {
      console.error('Error fetching categories:', error);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);

    try {
      const data = new FormData();
      data.append('name_Ar', formData.name_Ar);
      data.append('name_En', formData.name_En);
      data.append('description', formData.description);
      data.append('price', formData.price);
      if (formData.discountedPrice) {
        data.append('discountedPrice', formData.discountedPrice);
      }
      data.append('stock', formData.stock);
      data.append('categoryId', formData.categoryId);
      if (formData.brand) data.append('brand', formData.brand);
      if (formData.countryOfOrigin) data.append('countryOfOrigin', formData.countryOfOrigin);
      if (formData.warranty) data.append('warranty', formData.warranty);

      // إضافة الصور الجديدة
      if (images) {
        for (let i = 0; i < images.length; i++) {
          data.append('images', images[i]);
        }
      }

      // إضافة IDs الصور المراد حذفها
      imagesToDelete.forEach(id => {
        data.append('imagesToDelete', id.toString());
      });

      await api.put(`/products/${productId}`, data, {
        headers: { 'Content-Type': 'multipart/form-data' },
      });

      toast.success('تم تحديث المنتج بنجاح');
      router.push('/admin/products');
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'حدث خطأ');
    } finally {
      setSaving(false);
    }
  };

  const deleteImage = (imageId: number) => {
    setImagesToDelete([...imagesToDelete, imageId]);
    setExistingImages(existingImages.filter((img: any) => img.id !== imageId));
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50">
        <Navbar />
        <div className="max-w-4xl mx-auto px-4 py-8">
          <div className="text-center py-12">
            <p className="text-gray-600">جاري التحميل...</p>
          </div>
        </div>
        <Footer />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />

      <div className="max-w-4xl mx-auto px-4 py-8">
        <div className="flex justify-between items-center mb-8">
          <h1 className="text-3xl font-bold text-gray-800">تعديل المنتج</h1>
          <Link
            href="/admin/products"
            className="text-blue-600 hover:text-blue-700 flex items-center gap-2"
          >
            <FiArrowRight />
            العودة لقائمة المنتجات
          </Link>
        </div>

        <form onSubmit={handleSubmit} className="bg-white rounded-lg shadow-md p-6 space-y-6">
          {/* اسم المنتج */}
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                الاسم بالعربي *
              </label>
              <input
                type="text"
                required
                value={formData.name_Ar}
                onChange={(e) => setFormData({ ...formData, name_Ar: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                الاسم بالإنجليزي *
              </label>
              <input
                type="text"
                required
                value={formData.name_En}
                onChange={(e) => setFormData({ ...formData, name_En: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>

          {/* الوصف */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              الوصف *
            </label>
            <textarea
              required
              value={formData.description}
              onChange={(e) => setFormData({ ...formData, description: e.target.value })}
              rows={4}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          {/* السعر والخصم */}
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                السعر الأصلي *
              </label>
              <input
                type="number"
                required
                value={formData.price}
                onChange={(e) => setFormData({ ...formData, price: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                سعر الخصم (اختياري)
              </label>
              <input
                type="number"
                value={formData.discountedPrice}
                onChange={(e) => setFormData({ ...formData, discountedPrice: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>

          {/* المخزون والفئة */}
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                المخزون *
              </label>
              <input
                type="number"
                required
                value={formData.stock}
                onChange={(e) => setFormData({ ...formData, stock: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                الفئة *
              </label>
              <select
                required
                value={formData.categoryId}
                onChange={(e) => setFormData({ ...formData, categoryId: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="">اختر الفئة</option>
                {categories.map((cat: any) => (
                  <option key={cat.id} value={cat.id}>
                    {cat.name}
                  </option>
                ))}
              </select>
            </div>
          </div>

          {/* الماركة وبلد المنشأ والضمان */}
          <div className="grid grid-cols-3 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                الماركة
              </label>
              <input
                type="text"
                value={formData.brand}
                onChange={(e) => setFormData({ ...formData, brand: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                بلد المنشأ
              </label>
              <input
                type="text"
                value={formData.countryOfOrigin}
                onChange={(e) => setFormData({ ...formData, countryOfOrigin: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                الضمان
              </label>
              <input
                type="text"
                value={formData.warranty}
                onChange={(e) => setFormData({ ...formData, warranty: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="مثال: سنة واحدة"
              />
            </div>
          </div>

          {/* الصور الموجودة */}
          {existingImages.length > 0 && (
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                الصور الحالية
              </label>
              <div className="grid grid-cols-4 gap-4">
                {existingImages.map((img: any) => (
                  <div key={img.id} className="relative group">
                    <div className="relative w-full h-32 border rounded-lg overflow-hidden">
                      <Image
                        src={img.imageUrl}
                        alt="Product"
                        fill
                        className="object-cover"
                      />
                    </div>
                    <button
                      type="button"
                      onClick={() => deleteImage(img.id)}
                      className="absolute top-2 right-2 bg-red-500 text-white p-1 rounded opacity-0 group-hover:opacity-100 transition"
                    >
                      ×
                    </button>
                  </div>
                ))}
              </div>
            </div>
          )}

          {/* إضافة صور جديدة */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              إضافة صور جديدة
            </label>
            <input
              type="file"
              multiple
              accept="image/*"
              onChange={(e) => setImages(e.target.files)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <p className="text-xs text-gray-500 mt-1">
              يمكنك اختيار أكثر من صورة
            </p>
          </div>

          {/* الأزرار */}
          <div className="flex gap-4 pt-4">
            <button
              type="submit"
              disabled={saving}
              className="flex-1 bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 disabled:bg-gray-400"
            >
              {saving ? 'جاري الحفظ...' : 'حفظ التغييرات'}
            </button>
            <Link
              href="/admin/products"
              className="flex-1 bg-gray-200 text-gray-800 py-2 rounded-lg hover:bg-gray-300 text-center"
            >
              إلغاء
            </Link>
          </div>
        </form>
      </div>

      <Footer />
    </div>
  );
}

