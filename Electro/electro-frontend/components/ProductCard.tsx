'use client';

import Link from 'next/link';
import Image from 'next/image';
import { useRouter } from 'next/navigation';
import { FiHeart, FiShoppingCart } from 'react-icons/fi';
import { useCart } from '@/contexts/CartContext';
import { useState, useMemo, memo, useCallback } from 'react';
import toast from 'react-hot-toast';
import api from '@/lib/api';

interface Product {
  id: number;
  name: string;
  name_Ar?: string;
  name_En?: string;
  price: number;
  discountedPrice?: number;
  effectivePrice?: number;
  hasDiscount?: boolean;
  firstImageUrl?: string;
  imageUrl?: string;
  images?: Array<{ id: number; imageUrl: string }>;
  isFavorite?: boolean;
  stock?: number;
}

const ProductCard = memo(function ProductCard({ product }: { product: Product }) {
  const { addToCart } = useCart();
  const [isAddingToCart, setIsAddingToCart] = useState(false);
  const [isFavorite, setIsFavorite] = useState<boolean>(Boolean(product.isFavorite));
  const router = useRouter();

  const handleAddToCart = useCallback(async (e: React.MouseEvent) => {
    e.preventDefault();
    setIsAddingToCart(true);
    try {
      await addToCart(product.id, 1);
    } catch (error) {
      // Error handled in context
    } finally {
      setIsAddingToCart(false);
    }
  }, [product.id, addToCart]);

  const handleToggleFavorite = useCallback(async (e: React.MouseEvent) => {
    e.preventDefault();

    // تأكد من تسجيل الدخول
    if (typeof window !== 'undefined' && !localStorage.getItem('token')) {
      toast.error('يرجى تسجيل الدخول لإضافة المنتجات للمفضلة');
      router.push('/login');
      return;
    }

    try {
      if (!isFavorite) {
        // API الباك إند متوقع POST على /favorites/{productId}
        await api.post(`/favorites/${product.id}`);
        setIsFavorite(true);
        toast.success('تم الإضافة للمفضلة');
      } else {
        await api.delete(`/favorites/${product.id}`);
        setIsFavorite(false);
        toast.success('تم الحذف من المفضلة');
      }
    } catch (error: any) {
      if (error?.response?.status === 401) {
        toast.error('انتهت صلاحية الجلسة، يرجى تسجيل الدخول مرة أخرى');
        router.push('/login');
        return;
      }
      toast.error(error?.response?.data?.message || 'حدث خطأ أثناء تحديث المفضلة');
      console.error('Favorite toggle error:', error);
    }
  }, [isFavorite, product.id, router]);

  // Get image URL - prioritize firstImageUrl, then imageUrl, then first image from images array
  const imageUrl = useMemo(() => 
    product.firstImageUrl || product.imageUrl || 
    (product.images && product.images.length > 0 ? product.images[0].imageUrl : '/placeholder.png'),
    [product.firstImageUrl, product.imageUrl, product.images]
  );
  
  // Get product name
  const productName = useMemo(() => 
    product.name || product.name_Ar || product.name_En || 'منتج',
    [product.name, product.name_Ar, product.name_En]
  );
  
  // Calculate discount
  const discount = useMemo(() => 
    product.hasDiscount && product.effectivePrice
      ? Math.round(((product.price - Number(product.effectivePrice)) / product.price) * 100)
      : product.discountedPrice
      ? Math.round(((product.price - product.discountedPrice) / product.price) * 100)
      : 0,
    [product.hasDiscount, product.effectivePrice, product.price, product.discountedPrice]
  );
  
  const displayPrice = useMemo(() => 
    product.effectivePrice ? Number(product.effectivePrice) : 
    (product.discountedPrice || product.price),
    [product.effectivePrice, product.discountedPrice, product.price]
  );

  return (
    <Link href={`/products/${product.id}`}>
      <div className="bg-white rounded-lg shadow-md hover:shadow-xl transition-shadow duration-300 overflow-hidden group">
        {/* الصورة */}
        <div className="relative h-64 bg-gray-100">
          <Image
            src={imageUrl}
            alt={productName}
            fill
            className="object-cover group-hover:scale-105 transition-transform duration-300"
            loading="lazy"
            sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 33vw"
            unoptimized={imageUrl.startsWith('http://localhost:5008')}
          />
          
          {/* شارة الخصم */}
          {discount > 0 && (
            <div className="absolute top-2 right-2 bg-red-500 text-white px-2 py-1 rounded-lg text-sm font-semibold">
              {discount}%-
            </div>
          )}
          
          {/* زر المفضلة */}
          <button
            className="absolute top-2 left-2 bg-white p-2 rounded-full shadow-md hover:bg-primary-50 transition"
            onClick={handleToggleFavorite}
          >
            <FiHeart
              className={isFavorite ? 'text-red-500 fill-red-500' : 'text-gray-600'}
            />
          </button>
        </div>

        {/* المحتوى */}
        <div className="p-4">
          <h3 className="text-lg font-semibold text-gray-800 mb-2 line-clamp-2">
            {productName}
          </h3>
          
          <div className="flex items-center justify-between mb-3">
            <div className="flex flex-col">
              {discount > 0 ? (
                <>
                  <span className="text-xl font-bold text-primary-600">
                    {displayPrice} جنيه
                  </span>
                  <span className="text-sm text-gray-500 line-through">
                    {product.price} جنيه
                  </span>
                </>
              ) : (
                <span className="text-xl font-bold text-primary-600">
                  {product.price} جنيه
                </span>
              )}
            </div>
          </div>

          {/* زر الإضافة للسلة */}
          <button
            onClick={handleAddToCart}
            disabled={isAddingToCart || product.stock === 0}
            className="w-full bg-primary-600 text-white py-2 rounded-lg hover:bg-primary-700 transition flex items-center justify-center space-x-2 space-x-reverse disabled:bg-gray-400"
          >
            <FiShoppingCart />
            <span>{isAddingToCart ? 'جاري الإضافة...' : 'أضف للسلة'}</span>
          </button>
          
          {product.stock === 0 && (
            <p className="text-red-500 text-sm text-center mt-2">غير متوفر</p>
          )}
        </div>
      </div>
    </Link>
  );
});

export default ProductCard;

