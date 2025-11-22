import Link from 'next/link';
import { FiFacebook, FiTwitter, FiInstagram, FiMail, FiPhone, FiMapPin } from 'react-icons/fi';

export default function Footer() {
  return (
    <footer className="bg-gray-900 text-gray-300">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
          {/* معلومات المتجر */}
          <div>
            <h3 className="text-white text-xl font-bold mb-4">Electro Medical</h3>
            <p className="text-gray-400 mb-4">
              متجرك الموثوق للأجهزة والمعدات الطبية. نوفر أجود المنتجات الطبية: أسرة طبية، أجهزة قياس الضغط والسكر، كراسي متحركة، ومستلزمات طبية متنوعة بأفضل الأسعار.
            </p>
            <div className="flex space-x-4 space-x-reverse">
              <a href="#" className="hover:text-primary-500 transition">
                <FiFacebook size={20} />
              </a>
              <a href="#" className="hover:text-primary-500 transition">
                <FiTwitter size={20} />
              </a>
              <a href="#" className="hover:text-primary-500 transition">
                <FiInstagram size={20} />
              </a>
            </div>
          </div>

          {/* روابط سريعة */}
          <div>
            <h4 className="text-white font-semibold mb-4">روابط سريعة</h4>
            <ul className="space-y-2">
              <li>
                <Link href="/products" className="hover:text-primary-500 transition">
                  المنتجات
                </Link>
              </li>
              <li>
                <Link href="/categories" className="hover:text-primary-500 transition">
                  الفئات
                </Link>
              </li>
              <li>
                <Link href="/about" className="hover:text-primary-500 transition">
                  من نحن
                </Link>
              </li>
              <li>
                <Link href="/contact" className="hover:text-primary-500 transition">
                  اتصل بنا
                </Link>
              </li>
            </ul>
          </div>

          {/* خدمة العملاء */}
          <div>
            <h4 className="text-white font-semibold mb-4">خدمة العملاء</h4>
            <ul className="space-y-2">
              <li>
                <Link href="/orders" className="hover:text-primary-500 transition">
                  تتبع الطلب
                </Link>
              </li>
              <li>
                <Link href="/return-policy" className="hover:text-primary-500 transition">
                  سياسة الإرجاع
                </Link>
              </li>
              <li>
                <Link href="/shipping" className="hover:text-primary-500 transition">
                  الشحن والتوصيل
                </Link>
              </li>
              <li>
                <Link href="/faq" className="hover:text-primary-500 transition">
                  الأسئلة الشائعة
                </Link>
              </li>
            </ul>
          </div>

          {/* معلومات التواصل */}
          <div>
            <h4 className="text-white font-semibold mb-4">تواصل معنا</h4>
            <ul className="space-y-3">
              <li className="flex items-start space-x-3 space-x-reverse">
                <FiMapPin className="mt-1 flex-shrink-0" />
                <span>القاهرة، مصر</span>
              </li>
              <li className="flex items-center space-x-3 space-x-reverse">
                <FiPhone className="flex-shrink-0" />
                <span>+20 123 456 7890</span>
              </li>
              <li className="flex items-center space-x-3 space-x-reverse">
                <FiMail className="flex-shrink-0" />
                <span>info@electro.com</span>
              </li>
            </ul>
          </div>
        </div>

        {/* حقوق النشر */}
        <div className="border-t border-gray-800 mt-8 pt-8 text-center text-gray-400">
          <p>© 2024 Electro. جميع الحقوق محفوظة.</p>
        </div>
      </div>
    </footer>
  );
}

