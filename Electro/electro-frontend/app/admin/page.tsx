'use client';

import { useState, useEffect } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { useRouter } from 'next/navigation';
import Navbar from '@/components/Navbar';
import Footer from '@/components/Footer';
import api from '@/lib/api';
import Link from 'next/link';
import toast from 'react-hot-toast';
import { FiPackage, FiShoppingBag, FiUsers, FiDollarSign, FiTrendingUp } from 'react-icons/fi';

export default function AdminDashboardPage() {
  const { user, loading: authLoading } = useAuth();
  const router = useRouter();
  const [stats, setStats] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (authLoading) {
      return; // انتظر حتى يتم تحميل بيانات المستخدم
    }
    
    if (!user) {
      router.push('/login');
      return;
    }
    
    // التحقق من Admin role - دعم عدة أشكال
    const isAdmin = 
      user.roles?.includes('Admin') || 
      user.roles?.includes('admin') ||
      user.role === 'Admin' || 
      user.role === 'admin' ||
      user.roles?.some((r: string) => r?.toLowerCase() === 'admin');
    
    if (!isAdmin) {
      console.log('❌ ليس لديك صلاحيات أدمن');
      console.log('User data:', { roles: user.roles, role: user.role, email: user.email });
      toast.error('ليس لديك صلاحيات للوصول إلى لوحة التحكم');
      router.push('/');
      return;
    }
    
    console.log('✅ تم التحقق من صلاحيات الأدمن بنجاح');
    fetchDashboardStats();
  }, [user, authLoading, router]);

  const fetchDashboardStats = async () => {
    try {
      const response = await api.get('/admindashboard');
      setStats(response.data.data);
    } catch (error) {
      console.error('Error fetching dashboard stats:', error);
    } finally {
      setLoading(false);
    }
  };

  if (authLoading || loading) {
    return (
      <div className="min-h-screen bg-gray-50">
        <Navbar />
        <div className="max-w-7xl mx-auto px-4 py-8">
          <div className="animate-pulse space-y-6">
            <div className="h-32 bg-white rounded-lg"></div>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              {[...Array(3)].map((_, i) => (
                <div key={i} className="h-32 bg-white rounded-lg"></div>
              ))}
            </div>
          </div>
        </div>
        <Footer />
      </div>
    );
  }

  if (!user) {
    return null; // سيتم إعادة التوجيه في useEffect
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />

      <div className="max-w-7xl mx-auto px-4 py-8">
        <h1 className="text-3xl font-bold mb-8">لوحة التحكم</h1>

        {/* الإحصائيات */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
          <div className="bg-white rounded-lg shadow-md p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-gray-600 text-sm">إجمالي الطلبات</p>
                <p className="text-3xl font-bold text-primary-600">
                  {stats?.totalOrders || 0}
                </p>
              </div>
              <FiPackage size={48} className="text-primary-600 opacity-20" />
            </div>
          </div>

          <div className="bg-white rounded-lg shadow-md p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-gray-600 text-sm">إجمالي المنتجات</p>
                <p className="text-3xl font-bold text-green-600">
                  {stats?.totalProducts || 0}
                </p>
              </div>
              <FiShoppingBag size={48} className="text-green-600 opacity-20" />
            </div>
          </div>

          <div className="bg-white rounded-lg shadow-md p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-gray-600 text-sm">إجمالي المستخدمين</p>
                <p className="text-3xl font-bold text-blue-600">
                  {stats?.totalUsers || 0}
                </p>
              </div>
              <FiUsers size={48} className="text-blue-600 opacity-20" />
            </div>
          </div>

          <div className="bg-white rounded-lg shadow-md p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-gray-600 text-sm">إجمالي المبيعات</p>
                <p className="text-3xl font-bold text-purple-600">
                  {stats?.totalRevenue || 0} جنيه
                </p>
              </div>
              <FiDollarSign size={48} className="text-purple-600 opacity-20" />
            </div>
          </div>
        </div>

        {/* روابط سريعة */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <Link
            href="/admin/orders"
            className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow"
          >
            <div className="flex items-center gap-4">
              <div className="bg-primary-100 p-4 rounded-lg">
                <FiPackage size={32} className="text-primary-600" />
              </div>
              <div>
                <h3 className="text-xl font-semibold">إدارة الطلبات</h3>
                <p className="text-gray-600">عرض وتحديث الطلبات</p>
              </div>
            </div>
          </Link>

          <Link
            href="/admin/products"
            className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow"
          >
            <div className="flex items-center gap-4">
              <div className="bg-green-100 p-4 rounded-lg">
                <FiShoppingBag size={32} className="text-green-600" />
              </div>
              <div>
                <h3 className="text-xl font-semibold">إدارة المنتجات</h3>
                <p className="text-gray-600">إضافة وتعديل المنتجات</p>
              </div>
            </div>
          </Link>

          <Link
            href="/admin/categories"
            className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow"
          >
            <div className="flex items-center gap-4">
              <div className="bg-blue-100 p-4 rounded-lg">
                <FiTrendingUp size={32} className="text-blue-600" />
              </div>
              <div>
                <h3 className="text-xl font-semibold">إدارة الفئات</h3>
                <p className="text-gray-600">إضافة وتعديل الفئات</p>
              </div>
            </div>
          </Link>
        </div>
      </div>

      <Footer />
    </div>
  );
}

