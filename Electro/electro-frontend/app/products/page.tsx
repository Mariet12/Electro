'use client';

import { useState, useEffect } from 'react';
import { useSearchParams } from 'next/navigation';
import Navbar from '@/components/Navbar';
import Footer from '@/components/Footer';
import ProductCard from '@/components/ProductCard';
import api from '@/lib/api';
import { FiSliders } from 'react-icons/fi';

export default function ProductsPage() {
  const searchParams = useSearchParams();
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [filters, setFilters] = useState({
    categoryId: searchParams.get('categoryId') || '',
    search: searchParams.get('search') || '',
    brand: '',
    priceFrom: '',
    priceTo: '',
    countryOfOrigin: '',
    sortBy: 'new',
  });
  const [pagination, setPagination] = useState({
    pageNumber: 1,
    pageSize: 20,
    totalPages: 1,
    totalCount: 0,
  });
  const [showFilters, setShowFilters] = useState(false);
  const [categories, setCategories] = useState([]);

  useEffect(() => {
    fetchCategories();
  }, []);

  useEffect(() => {
    fetchProducts();
  }, [filters, pagination.pageNumber]);

  const fetchCategories = async () => {
    try {
      const response = await api.get('/category?pageSize=100');
      setCategories(response.data.data.items || []);
    } catch (error) {
      console.error('Error fetching categories:', error);
    }
  };

  const fetchProducts = async () => {
    setLoading(true);
    try {
      const params = new URLSearchParams();
      if (filters.categoryId) params.append('categoryId', filters.categoryId);
      if (filters.search) params.append('search', filters.search);
      if (filters.brand) params.append('brand', filters.brand);
      if (filters.priceFrom) params.append('priceFrom', filters.priceFrom);
      if (filters.priceTo) params.append('priceTo', filters.priceTo);
      if (filters.countryOfOrigin) params.append('countryOfOrigin', filters.countryOfOrigin);
      params.append('sortBy', filters.sortBy);
      params.append('pageNumber', pagination.pageNumber.toString());
      params.append('pageSize', pagination.pageSize.toString());

      const response = await api.get(`/products?${params.toString()}`);
      setProducts(response.data.data.items || []);
      setPagination({
        ...pagination,
        totalPages: response.data.data.totalPages || 1,
        totalCount: response.data.data.totalCount || 0,
      });
    } catch (error) {
      console.error('Error fetching products:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />

      <div className="max-w-7xl mx-auto px-4 py-8">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-3xl font-bold">المنتجات</h1>
          <button
            onClick={() => setShowFilters(!showFilters)}
            className="md:hidden flex items-center space-x-2 space-x-reverse bg-white px-4 py-2 rounded-lg shadow-md"
          >
            <FiSliders />
            <span>الفلاتر</span>
          </button>
        </div>

        <div className="flex flex-col md:flex-row gap-6">
          {/* الفلاتر */}
          <div className={`md:w-64 ${showFilters ? 'block' : 'hidden md:block'}`}>
            <div className="bg-white rounded-lg shadow-md p-6 space-y-6">
              <h3 className="font-semibold text-lg">الفلاتر</h3>

              {/* الترتيب */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  الترتيب
                </label>
                <select
                  value={filters.sortBy}
                  onChange={(e) => setFilters({ ...filters, sortBy: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                >
                  <option value="new">الأحدث</option>
                  <option value="price-asc">السعر: من الأقل للأعلى</option>
                  <option value="price-desc">السعر: من الأعلى للأقل</option>
                  <option value="name">الاسم</option>
                </select>
              </div>

              {/* الفئة */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  الفئة
                </label>
                <select
                  value={filters.categoryId}
                  onChange={(e) => setFilters({ ...filters, categoryId: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                >
                  <option value="">كل الفئات</option>
                  {categories.map((cat: any) => (
                    <option key={cat.id} value={cat.id}>
                      {cat.name}
                    </option>
                  ))}
                </select>
              </div>

              {/* السعر */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  نطاق السعر
                </label>
                <div className="flex gap-2">
                  <input
                    type="number"
                    placeholder="من"
                    value={filters.priceFrom}
                    onChange={(e) => setFilters({ ...filters, priceFrom: e.target.value })}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  />
                  <input
                    type="number"
                    placeholder="إلى"
                    value={filters.priceTo}
                    onChange={(e) => setFilters({ ...filters, priceTo: e.target.value })}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>
              </div>

              {/* الماركة */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  الماركة
                </label>
                <input
                  type="text"
                  value={filters.brand}
                  onChange={(e) => setFilters({ ...filters, brand: e.target.value })}
                  placeholder="أدخل اسم الماركة"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                />
              </div>

              {/* بلد المنشأ */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  بلد المنشأ
                </label>
                <input
                  type="text"
                  value={filters.countryOfOrigin}
                  onChange={(e) => setFilters({ ...filters, countryOfOrigin: e.target.value })}
                  placeholder="أدخل بلد المنشأ"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                />
              </div>

              <button
                onClick={() => {
                  setFilters({
                    categoryId: '',
                    search: '',
                    brand: '',
                    priceFrom: '',
                    priceTo: '',
                    countryOfOrigin: '',
                    sortBy: 'new',
                  });
                }}
                className="w-full text-primary-600 py-2 border border-primary-600 rounded-lg hover:bg-primary-50"
              >
                إعادة تعيين الفلاتر
              </button>
            </div>
          </div>

          {/* المنتجات */}
          <div className="flex-1">
            {loading ? (
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
                {[...Array(6)].map((_, i) => (
                  <div key={i} className="bg-white rounded-lg shadow-md h-96 animate-pulse"></div>
                ))}
              </div>
            ) : products.length > 0 ? (
              <>
                <div className="mb-4 text-gray-600">
                  عرض {products.length} من أصل {pagination.totalCount} منتج
                </div>
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
                  {products.map((product: any) => (
                    <ProductCard key={product.id} product={product} />
                  ))}
                </div>

                {/* الترقيم */}
                {pagination.totalPages > 1 && (
                  <div className="flex justify-center mt-8 gap-2">
                    <button
                      onClick={() => setPagination({ ...pagination, pageNumber: pagination.pageNumber - 1 })}
                      disabled={pagination.pageNumber === 1}
                      className="px-4 py-2 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50"
                    >
                      السابق
                    </button>
                    <span className="px-4 py-2 bg-primary-600 text-white rounded-lg">
                      {pagination.pageNumber} / {pagination.totalPages}
                    </span>
                    <button
                      onClick={() => setPagination({ ...pagination, pageNumber: pagination.pageNumber + 1 })}
                      disabled={pagination.pageNumber === pagination.totalPages}
                      className="px-4 py-2 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50"
                    >
                      التالي
                    </button>
                  </div>
                )}
              </>
            ) : (
              <div className="text-center py-12">
                <p className="text-gray-600 text-lg">لا توجد منتجات</p>
              </div>
            )}
          </div>
        </div>
      </div>

      <Footer />
    </div>
  );
}

