'use client';

import { useEffect, useState } from 'react';
import api from '@/lib/api';
import Link from 'next/link';
import Image from 'next/image';

export default function Categories() {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchCategories();
  }, []);

  const fetchCategories = async () => {
    try {
      const response = await api.get('/category?pageSize=6');
      setCategories(response.data.data.items || []);
    } catch (error) {
      console.error('Error fetching categories:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading || categories.length === 0) return null;

  return (
    <div className="py-16 bg-white">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <h2 className="text-3xl font-bold text-center mb-10 text-gray-800">تصفح حسب الفئة</h2>
        <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-4">
          {categories.map((category: any) => (
            <Link
              key={category.id}
              href={`/products?categoryId=${category.id}`}
              className="group"
            >
              <div className="bg-gray-50 rounded-lg p-4 hover:shadow-lg transition-shadow text-center">
                {category.imageUrl && (
                  <div className="relative h-24 mb-3">
                    <Image
                      src={category.imageUrl}
                      alt={category.name}
                      fill
                      className="object-contain"
                    />
                  </div>
                )}
                <h3 className="font-semibold text-gray-800 group-hover:text-primary-600 transition">
                  {category.name}
                </h3>
              </div>
            </Link>
          ))}
        </div>
      </div>
    </div>
  );
}

