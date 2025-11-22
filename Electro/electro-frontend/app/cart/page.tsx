'use client';

import { useEffect } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { useRouter } from 'next/navigation';
import { useCart } from '@/contexts/CartContext';
import Navbar from '@/components/Navbar';
import Footer from '@/components/Footer';
import Image from 'next/image';
import Link from 'next/link';
import { FiMinus, FiPlus, FiTrash2, FiShoppingBag } from 'react-icons/fi';

export default function CartPage() {
  const { user, loading: authLoading } = useAuth();
  const router = useRouter();
  const { cart, updateQuantity, removeItem, clearCart, loading } = useCart();

  useEffect(() => {
    if (!authLoading && !user) {
      router.push('/login');
    }
  }, [user, authLoading, router]);

  if (!cart || cart.items.length === 0) {
    return (
      <div className="min-h-screen bg-gray-50">
        <Navbar />
        <div className="max-w-7xl mx-auto px-4 py-16">
          <div className="text-center">
            <FiShoppingBag size={64} className="mx-auto text-gray-400 mb-4" />
            <h2 className="text-2xl font-bold text-gray-800 mb-2">السلة فارغة</h2>
            <p className="text-gray-600 mb-8">لم تقم بإضافة أي منتجات للسلة بعد</p>
            <Link
              href="/products"
              className="inline-block bg-primary-600 text-white px-8 py-3 rounded-lg hover:bg-primary-700"
            >
              تصفح المنتجات
            </Link>
          </div>
        </div>
        <Footer />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />

      <div className="max-w-7xl mx-auto px-4 py-8">
        <h1 className="text-3xl font-bold mb-8">سلة التسوق</h1>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* قائمة المنتجات */}
          <div className="lg:col-span-2">
            <div className="bg-white rounded-lg shadow-md">
              {cart.items.map((item) => (
                <div key={item.id} className="p-6 border-b border-gray-200 last:border-b-0">
                  <div className="flex gap-4">
                    {/* صورة المنتج */}
                    <div className="relative w-24 h-24 flex-shrink-0">
                      <Image
                        src={item.productImage || '/placeholder.png'}
                        alt={item.productName}
                        fill
                        className="object-cover rounded-lg"
                      />
                    </div>

                    {/* معلومات المنتج */}
                    <div className="flex-1">
                      <Link
                        href={`/products/${item.productId}`}
                        className="text-lg font-semibold text-gray-800 hover:text-primary-600"
                      >
                        {item.productName}
                      </Link>
                      <p className="text-gray-600 mt-1">{item.price} جنيه</p>

                      {/* التحكم في الكمية */}
                      <div className="flex items-center gap-4 mt-4">
                        <div className="flex items-center border border-gray-300 rounded-lg">
                          <button
                            onClick={() => updateQuantity(item.id, item.quantity - 1)}
                            disabled={loading || item.quantity <= 1}
                            className="p-2 hover:bg-gray-100 disabled:opacity-50"
                          >
                            <FiMinus size={16} />
                          </button>
                          <span className="px-4 font-semibold">{item.quantity}</span>
                          <button
                            onClick={() => updateQuantity(item.id, item.quantity + 1)}
                            disabled={loading}
                            className="p-2 hover:bg-gray-100 disabled:opacity-50"
                          >
                            <FiPlus size={16} />
                          </button>
                        </div>

                        <button
                          onClick={() => removeItem(item.id)}
                          disabled={loading}
                          className="text-red-600 hover:text-red-700 flex items-center gap-1"
                        >
                          <FiTrash2 />
                          <span>حذف</span>
                        </button>
                      </div>
                    </div>

                    {/* السعر الإجمالي */}
                    <div className="text-left">
                      <p className="text-xl font-bold text-primary-600">
                        {item.totalPrice} جنيه
                      </p>
                    </div>
                  </div>
                </div>
              ))}

              {/* زر إفراغ السلة */}
              <div className="p-6">
                <button
                  onClick={clearCart}
                  disabled={loading}
                  className="text-red-600 hover:text-red-700 flex items-center gap-2"
                >
                  <FiTrash2 />
                  <span>إفراغ السلة</span>
                </button>
              </div>
            </div>
          </div>

          {/* ملخص الطلب */}
          <div className="lg:col-span-1">
            <div className="bg-white rounded-lg shadow-md p-6 sticky top-24">
              <h2 className="text-xl font-bold mb-6">ملخص الطلب</h2>

              <div className="space-y-4 mb-6">
                <div className="flex justify-between">
                  <span className="text-gray-600">عدد المنتجات:</span>
                  <span className="font-semibold">{cart.itemsCount}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">المجموع الفرعي:</span>
                  <span className="font-semibold">{cart.totalAmount} جنيه</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">الشحن:</span>
                  <span className="font-semibold text-green-600">مجاني</span>
                </div>
                <div className="border-t border-gray-200 pt-4">
                  <div className="flex justify-between text-lg">
                    <span className="font-bold">الإجمالي:</span>
                    <span className="font-bold text-primary-600">
                      {cart.totalAmount} جنيه
                    </span>
                  </div>
                </div>
              </div>

              <Link
                href="/checkout"
                className="block w-full bg-primary-600 text-white text-center py-3 rounded-lg hover:bg-primary-700 font-semibold"
              >
                إتمام الطلب
              </Link>

              <Link
                href="/products"
                className="block w-full text-center text-primary-600 py-3 mt-3 hover:underline"
              >
                متابعة التسوق
              </Link>
            </div>
          </div>
        </div>
      </div>

      <Footer />
    </div>
  );
}

