'use client';

import { useState, useEffect } from 'react';
import { useParams, useRouter } from 'next/navigation';
import Navbar from '@/components/Navbar';
import Footer from '@/components/Footer';
import api from '@/lib/api';
import toast from 'react-hot-toast';
import Image from 'next/image';
import { FiPackage, FiClock, FiCheckCircle, FiXCircle, FiTruck, FiMapPin, FiPhone } from 'react-icons/fi';
import { format } from 'date-fns';
import { ar } from 'date-fns/locale';

const statusConfig: any = {
  Pending: { label: 'قيد الانتظار', color: 'text-yellow-600', bg: 'bg-yellow-50', icon: FiClock },
  Processing: { label: 'قيد المعالجة', color: 'text-blue-600', bg: 'bg-blue-50', icon: FiPackage },
  Shipped: { label: 'تم الشحن', color: 'text-purple-600', bg: 'bg-purple-50', icon: FiTruck },
  Delivered: { label: 'تم التوصيل', color: 'text-green-600', bg: 'bg-green-50', icon: FiCheckCircle },
  Cancelled: { label: 'ملغي', color: 'text-red-600', bg: 'bg-red-50', icon: FiXCircle },
};

export default function OrderDetailPage() {
  const params = useParams();
  const router = useRouter();
  const [order, setOrder] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const [cancelling, setCancelling] = useState(false);

  useEffect(() => {
    fetchOrder();
  }, [params.id]);

  const fetchOrder = async () => {
    try {
      const response = await api.get(`/orders/${params.id}`);
      setOrder(response.data.data);
    } catch (error) {
      console.error('Error fetching order:', error);
      toast.error('فشل تحميل الطلب');
    } finally {
      setLoading(false);
    }
  };

  const handleCancelOrder = async () => {
    if (!confirm('هل أنت متأكد من إلغاء هذا الطلب؟')) return;

    setCancelling(true);
    try {
      await api.put(`/orders/${params.id}/cancel`);
      toast.success('تم إلغاء الطلب');
      fetchOrder();
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'فشل إلغاء الطلب');
    } finally {
      setCancelling(false);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50">
        <Navbar />
        <div className="max-w-7xl mx-auto px-4 py-8">
          <div className="bg-white rounded-lg shadow-md h-96 animate-pulse"></div>
        </div>
        <Footer />
      </div>
    );
  }

  if (!order) {
    return (
      <div className="min-h-screen bg-gray-50">
        <Navbar />
        <div className="max-w-7xl mx-auto px-4 py-8">
          <div className="text-center py-12">
            <p className="text-gray-600 text-lg">الطلب غير موجود</p>
          </div>
        </div>
        <Footer />
      </div>
    );
  }

  const status = statusConfig[order.status] || statusConfig.Pending;
  const StatusIcon = status.icon;

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />

      <div className="max-w-7xl mx-auto px-4 py-8">
        <div className="mb-6">
          <button
            onClick={() => router.back()}
            className="text-primary-600 hover:text-primary-700"
          >
            ← العودة للطلبات
          </button>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {/* تفاصيل الطلب */}
          <div className="lg:col-span-2 space-y-6">
            {/* معلومات الطلب */}
            <div className="bg-white rounded-lg shadow-md p-6">
              <div className="flex justify-between items-start mb-6">
                <div>
                  <h1 className="text-2xl font-bold mb-2">طلب رقم #{order.id}</h1>
                  <p className="text-gray-600">
                    {order.createdAt && format(new Date(order.createdAt), 'PPP', { locale: ar })}
                  </p>
                </div>
                <div className={`flex items-center gap-2 ${status.bg} px-4 py-2 rounded-lg`}>
                  <StatusIcon className={status.color} />
                  <span className={`font-semibold ${status.color}`}>
                    {status.label}
                  </span>
                </div>
              </div>

              {/* المنتجات */}
              <div className="space-y-4">
                <h3 className="font-semibold text-lg">المنتجات</h3>
                {order.items?.map((item: any) => (
                  <div key={item.id} className="flex gap-4 p-4 bg-gray-50 rounded-lg">
                    <div className="relative w-20 h-20 flex-shrink-0">
                      <Image
                        src={item.productImage || '/placeholder.png'}
                        alt={item.productName}
                        fill
                        className="object-cover rounded-lg"
                      />
                    </div>
                    <div className="flex-1">
                      <h4 className="font-semibold">{item.productName}</h4>
                      <p className="text-gray-600 text-sm">الكمية: {item.quantity}</p>
                      <p className="text-primary-600 font-semibold">
                        {item.price} جنيه
                      </p>
                    </div>
                    <div className="text-left">
                      <p className="font-bold text-primary-600">
                        {item.totalPrice} جنيه
                      </p>
                    </div>
                  </div>
                ))}
              </div>
            </div>

            {/* معلومات الشحن */}
            <div className="bg-white rounded-lg shadow-md p-6">
              <h3 className="font-semibold text-lg mb-4">معلومات الشحن</h3>
              <div className="space-y-3">
                <div className="flex items-start gap-3">
                  <FiMapPin className="text-gray-400 mt-1" />
                  <div>
                    <p className="text-gray-600 text-sm">العنوان</p>
                    <p className="font-semibold">{order.shippingAddress}</p>
                  </div>
                </div>
                <div className="flex items-start gap-3">
                  <FiPhone className="text-gray-400 mt-1" />
                  <div>
                    <p className="text-gray-600 text-sm">رقم الهاتف</p>
                    <p className="font-semibold">{order.phoneNumber}</p>
                  </div>
                </div>
                {order.notes && (
                  <div className="mt-4 p-3 bg-blue-50 rounded-lg">
                    <p className="text-sm text-blue-800">
                      <strong>ملاحظات:</strong> {order.notes}
                    </p>
                  </div>
                )}
              </div>
            </div>
          </div>

          {/* ملخص الطلب والإجراءات */}
          <div className="lg:col-span-1 space-y-6">
            {/* ملخص المبلغ */}
            <div className="bg-white rounded-lg shadow-md p-6">
              <h3 className="font-semibold text-lg mb-4">ملخص المبلغ</h3>
              <div className="space-y-3">
                <div className="flex justify-between">
                  <span className="text-gray-600">المجموع الفرعي:</span>
                  <span className="font-semibold">{order.totalAmount} جنيه</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">الشحن:</span>
                  <span className="font-semibold text-green-600">مجاني</span>
                </div>
                <div className="border-t border-gray-200 pt-3">
                  <div className="flex justify-between text-lg">
                    <span className="font-bold">الإجمالي:</span>
                    <span className="font-bold text-primary-600">
                      {order.totalAmount} جنيه
                    </span>
                  </div>
                </div>
                <div className="pt-3">
                  <p className="text-sm text-gray-600">حالة الدفع:</p>
                  <p className={`font-semibold ${order.isPaid ? 'text-green-600' : 'text-orange-600'}`}>
                    {order.isPaid ? 'مدفوع' : 'غير مدفوع (الدفع عند الاستلام)'}
                  </p>
                </div>
              </div>
            </div>

            {/* الإجراءات */}
            {order.status === 'Pending' && (
              <button
                onClick={handleCancelOrder}
                disabled={cancelling}
                className="w-full bg-red-600 text-white py-3 rounded-lg hover:bg-red-700 disabled:bg-gray-400"
              >
                {cancelling ? 'جاري الإلغاء...' : 'إلغاء الطلب'}
              </button>
            )}
          </div>
        </div>
      </div>

      <Footer />
    </div>
  );
}

