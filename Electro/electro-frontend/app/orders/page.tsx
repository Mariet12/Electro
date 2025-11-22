'use client';

import { useState, useEffect } from 'react';
import Navbar from '@/components/Navbar';
import Footer from '@/components/Footer';
import api from '@/lib/api';
import Link from 'next/link';
import { FiPackage, FiClock, FiCheckCircle, FiXCircle, FiTruck } from 'react-icons/fi';
import { format } from 'date-fns';
import { ar } from 'date-fns/locale';

const statusConfig: any = {
  Pending: { label: 'قيد الانتظار', color: 'text-yellow-600', bg: 'bg-yellow-50', icon: FiClock },
  Processing: { label: 'قيد المعالجة', color: 'text-blue-600', bg: 'bg-blue-50', icon: FiPackage },
  Shipped: { label: 'تم الشحن', color: 'text-purple-600', bg: 'bg-purple-50', icon: FiTruck },
  Delivered: { label: 'تم التوصيل', color: 'text-green-600', bg: 'bg-green-50', icon: FiCheckCircle },
  Cancelled: { label: 'ملغي', color: 'text-red-600', bg: 'bg-red-50', icon: FiXCircle },
};

export default function OrdersPage() {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchOrders();
  }, []);

  const fetchOrders = async () => {
    try {
      const response = await api.get('/orders');
      setOrders(response.data.data || []);
    } catch (error) {
      console.error('Error fetching orders:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50">
        <Navbar />
        <div className="max-w-7xl mx-auto px-4 py-8">
          <div className="space-y-4">
            {[...Array(3)].map((_, i) => (
              <div key={i} className="bg-white rounded-lg shadow-md h-40 animate-pulse"></div>
            ))}
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
        <h1 className="text-3xl font-bold mb-8">طلباتي</h1>

        {orders.length === 0 ? (
          <div className="bg-white rounded-lg shadow-md p-12 text-center">
            <FiPackage size={64} className="mx-auto text-gray-400 mb-4" />
            <h2 className="text-2xl font-bold text-gray-800 mb-2">لا توجد طلبات</h2>
            <p className="text-gray-600 mb-8">لم تقم بإنشاء أي طلبات بعد</p>
            <Link
              href="/products"
              className="inline-block bg-primary-600 text-white px-8 py-3 rounded-lg hover:bg-primary-700"
            >
              تصفح المنتجات
            </Link>
          </div>
        ) : (
          <div className="space-y-4">
            {orders.map((order: any) => {
              const status = statusConfig[order.status] || statusConfig.Pending;
              const StatusIcon = status.icon;

              return (
                <Link
                  key={order.id}
                  href={`/orders/${order.id}`}
                  className="block bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow"
                >
                  <div className="p-6">
                    <div className="flex flex-col md:flex-row md:items-center justify-between mb-4">
                      <div>
                        <h3 className="text-lg font-semibold mb-1">
                          طلب رقم #{order.id}
                        </h3>
                        <p className="text-gray-600 text-sm">
                          {order.createdAt && format(new Date(order.createdAt), 'PPP', { locale: ar })}
                        </p>
                      </div>
                      <div className={`flex items-center gap-2 mt-2 md:mt-0 ${status.bg} px-4 py-2 rounded-lg`}>
                        <StatusIcon className={status.color} />
                        <span className={`font-semibold ${status.color}`}>
                          {status.label}
                        </span>
                      </div>
                    </div>

                    <div className="border-t border-gray-200 pt-4">
                      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                        <div>
                          <p className="text-gray-600 text-sm">عدد المنتجات</p>
                          <p className="font-semibold">{order.itemsCount || order.items?.length || 0}</p>
                        </div>
                        <div>
                          <p className="text-gray-600 text-sm">المجموع</p>
                          <p className="font-semibold text-primary-600">{order.totalAmount} جنيه</p>
                        </div>
                        <div>
                          <p className="text-gray-600 text-sm">حالة الدفع</p>
                          <p className={`font-semibold ${order.isPaid ? 'text-green-600' : 'text-orange-600'}`}>
                            {order.isPaid ? 'مدفوع' : 'غير مدفوع'}
                          </p>
                        </div>
                      </div>
                    </div>
                  </div>
                </Link>
              );
            })}
          </div>
        )}
      </div>

      <Footer />
    </div>
  );
}

