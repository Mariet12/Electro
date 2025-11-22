'use client';

import { useState, useEffect } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { useRouter } from 'next/navigation';
import Navbar from '@/components/Navbar';
import Footer from '@/components/Footer';
import api from '@/lib/api';
import toast from 'react-hot-toast';
import { format } from 'date-fns';
import { ar } from 'date-fns/locale';
import Link from 'next/link';

export default function AdminOrdersPage() {
  const { user } = useAuth();
  const router = useRouter();
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [filterStatus, setFilterStatus] = useState('');

  useEffect(() => {
    if (!user) {
      return;
    }
    
    const isAdmin = user.roles?.includes('Admin') || user.role === 'Admin' || user.roles?.some((r: string) => r.toLowerCase() === 'admin');
    
    if (!isAdmin) {
      router.push('/');
      return;
    }
    
    fetchOrders();
  }, [filterStatus, user, router]);

  const fetchOrders = async () => {
    setLoading(true);
    try {
      const params = new URLSearchParams();
      if (filterStatus) params.append('status', filterStatus);
      params.append('pageSize', '50');

      const response = await api.get(`/orders/admin/all?${params.toString()}`);
      setOrders(response.data.data.items || []);
    } catch (error) {
      console.error('Error fetching orders:', error);
    } finally {
      setLoading(false);
    }
  };

  const updateOrderStatus = async (orderId: number, newStatus: string) => {
    try {
      await api.put(`/orders/${orderId}/status`, { status: newStatus });
      toast.success('تم تحديث حالة الطلب');
      fetchOrders();
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'حدث خطأ');
    }
  };

  const togglePaymentStatus = async (orderId: number) => {
    try {
      await api.put(`/orders/${orderId}/toggle-payment`);
      toast.success('تم تحديث حالة الدفع');
      fetchOrders();
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'حدث خطأ');
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />

      <div className="max-w-7xl mx-auto px-4 py-8">
        <div className="flex justify-between items-center mb-8">
          <h1 className="text-3xl font-bold">إدارة الطلبات</h1>
          <Link href="/admin" className="text-primary-600 hover:text-primary-700">
            ← العودة للوحة التحكم
          </Link>
        </div>

        {/* الفلاتر */}
        <div className="bg-white rounded-lg shadow-md p-6 mb-6">
          <div className="flex gap-4">
            <select
              value={filterStatus}
              onChange={(e) => setFilterStatus(e.target.value)}
              className="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-primary-500 focus:border-primary-500"
            >
              <option value="">كل الطلبات</option>
              <option value="Pending">قيد الانتظار</option>
              <option value="Processing">قيد المعالجة</option>
              <option value="Shipped">تم الشحن</option>
              <option value="Delivered">تم التوصيل</option>
              <option value="Cancelled">ملغي</option>
            </select>
          </div>
        </div>

        {/* جدول الطلبات */}
        <div className="bg-white rounded-lg shadow-md overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    رقم الطلب
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    التاريخ
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    العميل
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    المبلغ
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    الحالة
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    الدفع
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    الإجراءات
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200">
                {loading ? (
                  <tr>
                    <td colSpan={7} className="px-6 py-4 text-center">
                      جاري التحميل...
                    </td>
                  </tr>
                ) : orders.length === 0 ? (
                  <tr>
                    <td colSpan={7} className="px-6 py-4 text-center text-gray-600">
                      لا توجد طلبات
                    </td>
                  </tr>
                ) : (
                  orders.map((order: any) => (
                    <tr key={order.id} className="hover:bg-gray-50">
                      <td className="px-6 py-4 whitespace-nowrap">
                        #{order.id}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                        {order.createdAt && format(new Date(order.createdAt), 'PP', { locale: ar })}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm">
                        {order.userName || 'N/A'}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap font-semibold text-primary-600">
                        {order.totalAmount} جنيه
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <select
                          value={order.status}
                          onChange={(e) => updateOrderStatus(order.id, e.target.value)}
                          className="text-sm px-2 py-1 border border-gray-300 rounded-lg focus:outline-none focus:ring-primary-500"
                        >
                          <option value="Pending">قيد الانتظار</option>
                          <option value="Processing">قيد المعالجة</option>
                          <option value="Shipped">تم الشحن</option>
                          <option value="Delivered">تم التوصيل</option>
                          <option value="Cancelled">ملغي</option>
                        </select>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <button
                          onClick={() => togglePaymentStatus(order.id)}
                          className={`text-sm px-3 py-1 rounded-lg ${
                            order.isPaid
                              ? 'bg-green-100 text-green-800'
                              : 'bg-orange-100 text-orange-800'
                          }`}
                        >
                          {order.isPaid ? 'مدفوع' : 'غير مدفوع'}
                        </button>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <Link
                          href={`/orders/${order.id}`}
                          className="text-primary-600 hover:text-primary-700"
                        >
                          عرض
                        </Link>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <Footer />
    </div>
  );
}

