'use client';

import Link from 'next/link';
import { useState, useMemo, useCallback, memo } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { useCart } from '@/contexts/CartContext';
import { FiShoppingCart, FiUser, FiHeart, FiMenu, FiX, FiSearch, FiBell, FiMessageCircle } from 'react-icons/fi';

export default function Navbar() {
  const { user, logout } = useAuth();
  const { cart } = useCart();
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');

  const handleSearch = useCallback((e: React.FormEvent) => {
    e.preventDefault();
    if (searchQuery.trim()) {
      window.location.href = `/products?search=${searchQuery}`;
    }
  }, [searchQuery]);

  const isAdmin = useMemo(() => 
    user?.roles?.includes('Admin') || 
    user?.role === 'Admin' || 
    user?.roles?.some((r: string) => r?.toLowerCase() === 'admin'),
    [user?.roles, user?.role]
  );

  return (
    <nav className="bg-white shadow-md sticky top-0 z-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-16">
          {/* اللوجو */}
          <Link href="/" className="flex items-center space-x-2 space-x-reverse">
            <span className="text-2xl font-bold text-blue-700">Electro Medical</span>
          </Link>

          {/* البحث */}
          <form onSubmit={handleSearch} className="hidden md:flex flex-1 max-w-md mx-8">
            <div className="relative w-full">
              <input
                type="text"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                placeholder="ابحث عن منتج..."
                className="w-full px-4 py-2 pr-10 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500"
              />
              <FiSearch className="absolute left-3 top-3 text-gray-400" />
            </div>
          </form>

          {/* القائمة الرئيسية */}
          <div className="hidden md:flex items-center space-x-6 space-x-reverse">
            <Link href="/products" className="text-gray-700 hover:text-primary-600 transition">
              المنتجات
            </Link>
            <Link href="/categories" className="text-gray-700 hover:text-primary-600 transition">
              الفئات
            </Link>
            
            {user ? (
              <>
                <Link href="/favorites" className="relative text-gray-700 hover:text-primary-600">
                  <FiHeart size={22} />
                </Link>
                
                <Link href="/notifications" className="relative text-gray-700 hover:text-primary-600">
                  <FiBell size={22} />
                </Link>
                
                <Link href="/chat" className="relative text-gray-700 hover:text-primary-600">
                  <FiMessageCircle size={22} />
                </Link>
                
                <Link href="/cart" className="relative text-gray-700 hover:text-primary-600">
                  <FiShoppingCart size={22} />
                  {cart && cart.itemsCount > 0 && (
                    <span className="absolute -top-2 -right-2 bg-red-500 text-white text-xs rounded-full h-5 w-5 flex items-center justify-center">
                      {cart.itemsCount}
                    </span>
                  )}
                </Link>
                
                <div className="relative">
                  <button 
                    className="flex items-center space-x-2 space-x-reverse text-gray-700 hover:text-primary-600"
                    onMouseEnter={() => {
                      setIsUserMenuOpen(true);
                    }}
                    onMouseLeave={() => {
                      // تأخير صغير قبل الإغلاق للسماح بالانتقال للقائمة
                      setTimeout(() => {
                        const dropdown = document.querySelector('.user-menu-dropdown');
                        const isHoveringDropdown = dropdown && dropdown.matches(':hover');
                        if (!isHoveringDropdown) {
                          setIsUserMenuOpen(false);
                        }
                      }, 150);
                    }}
                  >
                    <FiUser size={22} />
                    <span>{user.displayName}</span>
                  </button>
                  
                  {isUserMenuOpen && (
                    <div 
                      className="user-menu-dropdown absolute left-0 mt-2 w-48 bg-white rounded-lg shadow-lg py-2 z-50 border border-gray-200"
                      onMouseEnter={() => setIsUserMenuOpen(true)}
                      onMouseLeave={() => {
                        setTimeout(() => setIsUserMenuOpen(false), 150);
                      }}
                    >
                      <Link 
                        href="/profile" 
                        className="block px-4 py-2 text-gray-700 hover:bg-gray-100 transition"
                        onClick={() => setIsUserMenuOpen(false)}
                      >
                        الملف الشخصي
                      </Link>
                      <Link 
                        href="/orders" 
                        className="block px-4 py-2 text-gray-700 hover:bg-gray-100 transition"
                        onClick={() => setIsUserMenuOpen(false)}
                      >
                        طلباتي
                      </Link>
                      {isAdmin && (
                        <Link 
                          href="/admin" 
                          className="block px-4 py-2 text-gray-700 hover:bg-gray-100 transition"
                          onClick={() => setIsUserMenuOpen(false)}
                        >
                          لوحة التحكم
                        </Link>
                      )}
                      <button
                        onClick={() => {
                          setIsUserMenuOpen(false);
                          logout();
                        }}
                        className="block w-full text-right px-4 py-2 text-red-600 hover:bg-gray-100 transition"
                      >
                        تسجيل الخروج
                      </button>
                    </div>
                  )}
                </div>
              </>
            ) : (
              <>
                <Link
                  href="/login"
                  className="text-gray-700 hover:text-primary-600 transition"
                >
                  تسجيل الدخول
                </Link>
                <Link
                  href="/register"
                  className="bg-primary-600 text-white px-4 py-2 rounded-lg hover:bg-primary-700 transition"
                >
                  إنشاء حساب
                </Link>
              </>
            )}
          </div>

          {/* زر القائمة للموبايل */}
          <button
            onClick={() => setIsMenuOpen(!isMenuOpen)}
            className="md:hidden text-gray-700"
          >
            {isMenuOpen ? <FiX size={24} /> : <FiMenu size={24} />}
          </button>
        </div>

        {/* القائمة للموبايل */}
        {isMenuOpen && (
          <div className="md:hidden py-4 border-t">
            <form onSubmit={handleSearch} className="mb-4">
              <input
                type="text"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                placeholder="ابحث عن منتج..."
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500"
              />
            </form>
            
            <Link href="/products" className="block py-2 text-gray-700">
              المنتجات
            </Link>
            <Link href="/categories" className="block py-2 text-gray-700">
              الفئات
            </Link>
            
            {user ? (
              <>
                <Link href="/favorites" className="block py-2 text-gray-700">
                  المفضلة
                </Link>
                <Link href="/chat" className="block py-2 text-gray-700">
                  الشات
                </Link>
                <Link href="/cart" className="block py-2 text-gray-700">
                  السلة ({cart?.itemsCount || 0})
                </Link>
                <Link href="/profile" className="block py-2 text-gray-700">
                  الملف الشخصي
                </Link>
                <Link href="/orders" className="block py-2 text-gray-700">
                  طلباتي
                </Link>
                {isAdmin && (
                  <Link href="/admin" className="block py-2 text-gray-700">
                    لوحة التحكم
                  </Link>
                )}
                <button onClick={logout} className="block w-full text-right py-2 text-red-600">
                  تسجيل الخروج
                </button>
              </>
            ) : (
              <>
                <Link href="/login" className="block py-2 text-gray-700">
                  تسجيل الدخول
                </Link>
                <Link href="/register" className="block py-2 text-primary-600 font-semibold">
                  إنشاء حساب
                </Link>
              </>
            )}
          </div>
        )}
      </div>
    </nav>
  );
}

