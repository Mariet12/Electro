'use client';

import { useState, useEffect } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { useRouter } from 'next/navigation';
import Navbar from '@/components/Navbar';
import Footer from '@/components/Footer';
import api from '@/lib/api';
import toast from 'react-hot-toast';
import { FiPlus, FiEdit2, FiTrash2, FiImage } from 'react-icons/fi';
import Link from 'next/link';

export default function AdminProductsPage() {
  const { user } = useAuth();
  const router = useRouter();
  const [products, setProducts] = useState([]);
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
    
    fetchProducts();
    fetchCategories();
  }, [user, router]);

  const fetchProducts = async () => {
    try {
      const response = await api.get('/products?pageSize=100');
      setProducts(response.data.data.items || []);
    } catch (error) {
      console.error('Error fetching products:', error);
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

  const deleteProduct = async (id: number) => {
    if (!confirm('هل أنت متأكد من حذف هذا المنتج؟')) return;

    try {
      await api.delete(`/products/${id}`);
      toast.success('تم حذف المنتج بنجاح');
      fetchProducts();
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'حدث خطأ');
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />

      <div className="max-w-7xl mx-auto px-4 py-8">
        <div className="flex justify-between items-center mb-8">
          <h1 className="text-3xl font-bold text-gray-800">إدارة المنتجات</h1>
          <div className="flex gap-4">
            <Link
              href="/admin"
              className="text-blue-600 hover:text-blue-700"
            >
              ← العودة للوحة التحكم
            </Link>
            <button
              onClick={() => setShowAddModal(true)}
              className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 flex items-center gap-2"
            >
              <FiPlus />
              <span>إضافة منتج جديد</span>
            </button>
          </div>
        </div>

        {loading ? (
          <div className="text-center py-12">
            <p className="text-gray-600">جاري التحميل...</p>
          </div>
        ) : (
          <div className="bg-white rounded-lg shadow-md overflow-hidden">
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead className="bg-gray-50 border-b">
                  <tr>
                    <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                      الصورة
                    </th>
                    <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                      اسم المنتج
                    </th>
                    <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                      السعر
                    </th>
                    <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                      المخزون
                    </th>
                    <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                      الفئة
                    </th>
                    <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                      الإجراءات
                    </th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-200">
                  {products.length === 0 ? (
                    <tr>
                      <td colSpan={6} className="px-6 py-8 text-center text-gray-600">
                        لا توجد منتجات. اضغط &quot;إضافة منتج جديد&quot; للبدء!
                      </td>
                    </tr>
                  ) : (
                    products.map((product: any) => (
                      <tr key={product.id} className="hover:bg-gray-50">
                        <td className="px-6 py-4">
                          {(() => {
                            // محاولة الحصول على الصورة من عدة مصادر
                            const imageUrl = 
                              product.firstImageUrl || 
                              product.imageUrl || 
                              (product.images && product.images.length > 0 ? product.images[0].imageUrl : null) ||
                              (product.productImages && product.productImages.length > 0 ? product.productImages[0].imageUrl : null);
                            
                            return imageUrl ? (
                              <div className="relative w-16 h-16">
                                <img
                                  src={imageUrl}
                                  alt={product.name || product.name_Ar || 'Product'}
                                  className="w-16 h-16 object-cover rounded"
                                  onError={(e) => {
                                    // إذا فشل تحميل الصورة، عرض placeholder
                                    const target = e.currentTarget;
                                    target.style.display = 'none';
                                    const placeholder = target.parentElement?.querySelector('.placeholder');
                                    if (placeholder) {
                                      placeholder.classList.remove('hidden');
                                    }
                                  }}
                                />
                                <div className="w-16 h-16 bg-gray-200 rounded flex items-center justify-center hidden placeholder">
                                  <FiImage className="text-gray-400" size={20} />
                                </div>
                              </div>
                            ) : (
                              <div className="w-16 h-16 bg-gray-200 rounded flex items-center justify-center">
                                <FiImage className="text-gray-400" size={20} />
                              </div>
                            );
                          })()}
                        </td>
                        <td className="px-6 py-4">
                          <div className="text-sm font-medium text-gray-900">
                            {product.name_Ar || product.name_En || product.name || 'بدون اسم'}
                          </div>
                        </td>
                        <td className="px-6 py-4 text-sm text-gray-900">
                          {product.discountedPrice ? (
                            <div>
                              <span className="font-bold text-green-600">
                                {product.discountedPrice} جنيه
                              </span>
                              <br />
                              <span className="text-gray-500 line-through text-xs">
                                {product.price} جنيه
                              </span>
                            </div>
                          ) : (
                            <span className="font-bold">{product.price} جنيه</span>
                          )}
                        </td>
                        <td className="px-6 py-4 text-sm text-gray-900">
                          {product.stock}
                        </td>
                        <td className="px-6 py-4 text-sm text-gray-900">
                          {product.categoryName || 'غير محدد'}
                        </td>
                        <td className="px-6 py-4 text-sm">
                          <div className="flex gap-2">
                            <Link
                              href={`/admin/products/edit/${product.id}`}
                              className="text-blue-600 hover:text-blue-700"
                            >
                              <FiEdit2 size={18} />
                            </Link>
                            <button
                              onClick={() => deleteProduct(product.id)}
                              className="text-red-600 hover:text-red-700"
                            >
                              <FiTrash2 size={18} />
                            </button>
                          </div>
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          </div>
        )}

        {/* Modal إضافة منتج */}
        {showAddModal && (
          <AddProductModal
            categories={categories}
            onClose={() => setShowAddModal(false)}
            onSuccess={() => {
              setShowAddModal(false);
              fetchProducts();
            }}
          />
        )}
      </div>

      <Footer />
    </div>
  );
}

// مكون Modal لإضافة منتج
function AddProductModal({
  categories,
  onClose,
  onSuccess,
}: {
  categories: any[];
  onClose: () => void;
  onSuccess: () => void;
}) {
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    price: '',
    discountedPrice: '',
    stock: '',
    categoryId: '',
    brand: '',
    countryOfOrigin: '',
  });
  const [images, setImages] = useState<FileList | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      const data = new FormData();
      data.append('name', formData.name);
      data.append('description', formData.description);
      data.append('price', formData.price);
      if (formData.discountedPrice) {
        data.append('discountedPrice', formData.discountedPrice);
      }
      data.append('stock', formData.stock);
      data.append('categoryId', formData.categoryId);
      if (formData.brand) data.append('brand', formData.brand);
      if (formData.countryOfOrigin) data.append('countryOfOrigin', formData.countryOfOrigin);

      if (images) {
        for (let i = 0; i < images.length; i++) {
          data.append('images', images[i]);
        }
      }

      await api.post('/products', data, {
        headers: { 'Content-Type': 'multipart/form-data' },
      });

      toast.success('تم إضافة المنتج بنجاح');
      onSuccess();
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'حدث خطأ');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-lg max-w-2xl w-full max-h-[90vh] overflow-y-auto">
        <div className="p-6">
          <h2 className="text-2xl font-bold mb-6 text-gray-800">إضافة منتج جديد</h2>

          <form onSubmit={handleSubmit} className="space-y-4">
            {/* اسم المنتج */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                اسم المنتج *
              </label>
              <input
                type="text"
                required
                value={formData.name}
                onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900"
                placeholder="مثال: سماعة طبية Littmann"
              />
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
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900"
                placeholder="وصف تفصيلي للمنتج..."
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
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900"
                  placeholder="2500"
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
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900"
                  placeholder="2000"
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
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900"
                  placeholder="50"
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
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900"
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

            {/* الماركة وبلد المنشأ */}
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  الماركة
                </label>
                <input
                  type="text"
                  value={formData.brand}
                  onChange={(e) => setFormData({ ...formData, brand: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900"
                  placeholder="Littmann"
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
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900"
                  placeholder="ألمانيا"
                />
              </div>
            </div>

            {/* الصور */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                صور المنتج
              </label>
              <input
                type="file"
                multiple
                accept="image/*"
                onChange={(e) => setImages(e.target.files)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900"
              />
              <p className="text-xs text-gray-500 mt-1">
                يمكنك اختيار أكثر من صورة
              </p>
            </div>

            {/* الأزرار */}
            <div className="flex gap-4 pt-4">
              <button
                type="submit"
                disabled={loading}
                className="flex-1 bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 disabled:bg-gray-400"
              >
                {loading ? 'جاري الإضافة...' : 'إضافة المنتج'}
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
    </div>
  );
}

