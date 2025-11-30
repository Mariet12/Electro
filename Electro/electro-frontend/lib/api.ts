import axios from 'axios';

// في production على Vercel، استخدم Next.js API routes كـproxy
// في development، استخدم الـbackend مباشرة
const API_URL = 
  typeof window !== 'undefined' && window.location.hostname === 'localhost'
    ? (process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5008/api')
    : '/api'; // استخدم Next.js API routes في production

export const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// إضافة التوكن تلقائياً لكل طلب
api.interceptors.request.use(
  (config) => {
    if (typeof window !== 'undefined') {
      const token = localStorage.getItem('token');
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
    }
    
    // إذا كانت البيانات FormData، احذف Content-Type للسماح لـaxios بإضافته تلقائياً
    if (config.data instanceof FormData) {
      delete config.headers['Content-Type'];
    }
    
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// معالجة الأخطاء
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // إعادة توجيه لصفحة تسجيل الدخول
      if (typeof window !== 'undefined') {
        localStorage.removeItem('token');
        window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  }
);

export default api;

