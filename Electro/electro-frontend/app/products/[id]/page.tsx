'use client';

import { useState, useEffect, useMemo } from 'react';
import { useParams } from 'next/navigation';
import Navbar from '@/components/Navbar';
import Footer from '@/components/Footer';
import api from '@/lib/api';
import { useCart } from '@/contexts/CartContext';
import toast from 'react-hot-toast';
import Image from 'next/image';
import { FiHeart, FiShoppingCart, FiMinus, FiPlus } from 'react-icons/fi';

export default function ProductDetailPage() {
  const params = useParams();
  const { addToCart } = useCart();
  const [product, setProduct] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const [quantity, setQuantity] = useState(1);
  const [selectedImage, setSelectedImage] = useState(0);
  const [adding, setAdding] = useState(false);

  useEffect(() => {
    fetchProduct();
  }, [params.id]);

  const fetchProduct = async () => {
    try {
      const response = await api.get(`/products/${params.id}`);
      setProduct(response.data.data);
    } catch (error) {
      console.error('Error fetching product:', error);
    } finally {
      setLoading(false);
    }
  };

  const stockInfo = useMemo(() => {
    if (!product || typeof product.stock !== 'number') {
      return { value: null, isOutOfStock: false, max: Number.MAX_SAFE_INTEGER };
    }
    return {
      value: product.stock,
      isOutOfStock: product.stock <= 0,
      max: Math.max(1, product.stock),
    };
  }, [product]);

  const handleDecrease = () => {
    setQuantity((prev) => Math.max(1, prev - 1));
  };

  const handleIncrease = () => {
    setQuantity((prev) => {
      const next = prev + 1;
      return Math.min(stockInfo.max, next);
    });
  };

  const handleAddToCart = async () => {
    if (stockInfo.isOutOfStock) {
      toast.error('هذا المنتج غير متوفر حالياً');
      return;
    }

    setAdding(true);
    try {
      await addToCart(product.id, quantity);
    } catch (error) {
      // Error handled inside addToCart / interceptor
    } finally {
      setAdding(false);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50">
        <Navbar />
        <div className="max-w-7xl mx-auto px-4 py-8">
          <div className="animate-pulse">
            <div className="bg-white rounded-lg h-96"></div>
          </div>
        </div>
        <Footer />
      </div>
    );
  }

  if (!product) {
    return (
      <div className="min-h-screen bg-gray-50">
        <Navbar />
        <div className="max-w-7xl mx-auto px-4 py-8">
          <div className="text-center py-12">
            <p className="text-gray-600 text-lg">المنتج غير موجود</p>
          </div>
        </div>
        <Footer />
      </div>
    );
  }

  const discount = product.discountedPrice
    ? Math.round(((product.price - product.discountedPrice) / product.price) * 100)
    : 0;

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />

      <div className="max-w-7xl mx-auto px-4 py-8">
        <div className="bg-white rounded-lg shadow-md p-6 md:p-8">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
            {/* الصور */}
            <div>
              <div className="relative h-96 bg-gray-100 rounded-lg mb-4">
                <Image
                  src={
                    product.images?.[selectedImage]?.imageUrl || 
                    product.firstImageUrl || 
                    product.imageUrl || 
                    (product.images && product.images.length > 0 ? product.images[0].imageUrl : '/placeholder.png')
                  }
                  alt={product.name_Ar || product.name_En || product.name || 'منتج'}
                  fill
                  className="object-contain"
                  unoptimized={true}
                />
                {discount > 0 && (
                  <div className="absolute top-4 right-4 bg-red-500 text-white px-3 py-1 rounded-lg text-lg font-semibold">
                    {discount}%-
                  </div>
                )}
              </div>
              {product.images && product.images.length > 1 && (
                <div className="grid grid-cols-4 gap-2">
                  {product.images.map((img: any, idx: number) => {
                    const imgUrl = typeof img === 'string' ? img : img.imageUrl;
                    return (
                      <button
                        key={idx}
                        onClick={() => setSelectedImage(idx)}
                        className={`relative h-24 border-2 rounded-lg overflow-hidden ${
                          selectedImage === idx ? 'border-primary-600' : 'border-gray-200'
                        }`}
                      >
                        <Image 
                          src={imgUrl} 
                          alt={`${product.name_Ar || product.name_En || product.name || 'منتج'} ${idx + 1}`} 
                          fill 
                          className="object-cover"
                          unoptimized={true}
                        />
                      </button>
                    );
                  })}
                </div>
              )}
            </div>

            {/* المعلومات */}
            <div>
              <h1 className="text-3xl font-bold text-gray-900 mb-4">{product.name}</h1>
              
              <div className="mb-6">
                {product.discountedPrice ? (
                  <div className="flex items-center gap-4">
                    <span className="text-4xl font-bold text-primary-600">
                      {product.discountedPrice} جنيه
                    </span>
                    <span className="text-2xl text-gray-500 line-through">
                      {product.price} جنيه
                    </span>
                  </div>
                ) : (
                  <span className="text-4xl font-bold text-primary-600">
                    {product.price} جنيه
                  </span>
                )}
              </div>

              {/* معلومات إضافية */}
              <div className="border-t border-b border-gray-200 py-6 mb-6 space-y-3">
                {product.brand && (
                  <div className="flex justify-between">
                    <span className="text-gray-600">الماركة:</span>
                    <span className="font-semibold">{product.brand}</span>
                  </div>
                )}
                {product.countryOfOrigin && (
                  <div className="flex justify-between">
                    <span className="text-gray-600">بلد المنشأ:</span>
                    <span className="font-semibold">{product.countryOfOrigin}</span>
                  </div>
                )}
                <div className="flex justify-between">
                  <span className="text-gray-600">الحالة:</span>
                  {stockInfo.value === null ? (
                    <span className="font-semibold text-green-600">متوفر</span>
                  ) : stockInfo.isOutOfStock ? (
                    <span className="font-semibold text-red-600">غير متوفر</span>
                  ) : (
                    <span className="font-semibold text-green-600">
                      متوفر ({stockInfo.value})
                    </span>
                  )}
                </div>
              </div>

              {/* الكمية والإضافة للسلة */}
              {!stockInfo.isOutOfStock && (
                <div className="mb-6">
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    الكمية
                  </label>
                  <div className="flex items-center gap-4">
                    <div className="flex items-center border border-gray-300 rounded-lg">
                      <button
                        type="button"
                        onClick={handleDecrease}
                        className="p-3 hover:bg-gray-100"
                      >
                        <FiMinus />
                      </button>
                      <span className="px-6 py-2 font-semibold">{quantity}</span>
                      <button
                        type="button"
                        onClick={handleIncrease}
                        className="p-3 hover:bg-gray-100"
                      >
                        <FiPlus />
                      </button>
                    </div>
                    <button
                      onClick={handleAddToCart}
                      disabled={adding}
                      className="flex-1 bg-primary-600 text-white px-6 py-3 rounded-lg hover:bg-primary-700 flex items-center justify-center gap-2 disabled:bg-gray-400"
                    >
                      <FiShoppingCart />
                      <span>{adding ? 'جاري الإضافة...' : 'أضف للسلة'}</span>
                    </button>
                  </div>
                </div>
              )}

              {/* زر المفضلة */}
              <button
                onClick={() => toast.success('تم الإضافة للمفضلة')}
                className="w-full border-2 border-primary-600 text-primary-600 px-6 py-3 rounded-lg hover:bg-primary-50 flex items-center justify-center gap-2"
              >
                <FiHeart />
                <span>أضف للمفضلة</span>
              </button>

              {/* الوصف */}
              {product.description && (
                <div className="mt-8">
                  <h3 className="text-xl font-semibold mb-4">الوصف</h3>
                  <p className="text-gray-600 leading-relaxed">{product.description}</p>
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

