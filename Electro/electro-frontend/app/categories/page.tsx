'use client';

import { useState, useEffect } from 'react';
import Navbar from '@/components/Navbar';
import Footer from '@/components/Footer';
import api from '@/lib/api';
import Link from 'next/link';
import Image from 'next/image';

export default function CategoriesPage() {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchCategories();
  }, []);

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

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />

      <div className="max-w-7xl mx-auto px-4 py-8">
        <h1 className="text-3xl font-bold mb-8">جميع الفئات</h1>

        {loading ? (
          <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-6">
            {[...Array(12)].map((_, i) => (
              <div key={i} className="bg-white rounded-lg shadow-md h-40 animate-pulse"></div>
            ))}
          </div>
        ) : (
          <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-6">
            {categories.map((category: any) => (
              <Link
                key={category.id}
                href={`/products?categoryId=${category.id}`}
                className="bg-white rounded-lg shadow-md p-6 hover:shadow-xl transition-shadow text-center group"
              >
                {category.imageUrl && (
                  <div className="relative h-24 mb-4">
                    <Image
                      src={category.imageUrl}
                      alt={category.name}
                      fill
                      className="object-contain group-hover:scale-110 transition-transform"
                    />
                  </div>
                )}
                <h3 className="font-semibold text-gray-800 group-hover:text-primary-600 transition">
                  {category.name}
                </h3>
                {category.description && (
                  <p className="text-sm text-gray-600 mt-2 line-clamp-2">
                    {category.description}
                  </p>
                )}
              </Link>
            ))}
          </div>
        )}
      </div>

      <Footer />
    </div>
  );
}

