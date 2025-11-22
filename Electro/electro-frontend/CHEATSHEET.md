# 🎯 Cheat Sheet - Electro Frontend

مرجع سريع للأوامر والأكواد الأكثر استخداماً.

---

## ⚡ الأوامر السريعة

```bash
# تشغيل
npm run dev              # Development
npm run build           # Build للإنتاج
npm start               # تشغيل Production
npm run lint            # فحص الأخطاء

# تنظيف
rm -rf .next node_modules && npm install

# Port مختلف
PORT=3001 npm run dev
```

---

## 🔌 API Calls

```typescript
import api from '@/lib/api';

// GET
const { data } = await api.get('/products');

// POST
const { data } = await api.post('/cart/add', { productId: 1, quantity: 2 });

// PUT
const { data } = await api.put('/account/update-user', formData);

// DELETE
await api.delete('/cart/items/1');
```

---

## 🔐 Auth Context

```typescript
import { useAuth } from '@/contexts/AuthContext';

const { user, login, logout, register, updateUser } = useAuth();

// تسجيل الدخول
await login(email, password);

// إنشاء حساب
await register({ email, password, displayName, phoneNumber });

// تسجيل الخروج
logout();

// تحديث البيانات
await updateUser();
```

---

## 🛒 Cart Context

```typescript
import { useCart } from '@/contexts/CartContext';

const { cart, addToCart, updateQuantity, removeItem, clearCart, refreshCart } = useCart();

// إضافة للسلة
await addToCart(productId, quantity);

// تحديث الكمية
await updateQuantity(cartItemId, newQuantity);

// حذف منتج
await removeItem(cartItemId);

// تفريغ السلة
await clearCart();

// تحديث السلة
await refreshCart();

// عدد المنتجات
const count = cart?.itemsCount || 0;

// المجموع
const total = cart?.totalAmount || 0;
```

---

## 🎨 Tailwind Classes

```tsx
// أزرار
className="bg-primary-600 text-white px-4 py-2 rounded-lg hover:bg-primary-700"

// بطاقات
className="bg-white rounded-lg shadow-md p-6"

// Grid
className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6"

// Flex
className="flex items-center justify-between gap-4"

// Text
className="text-3xl font-bold text-gray-900"

// Spacing
className="mb-4 mt-2 px-6 py-3"

// Responsive
className="hidden md:block lg:flex"
```

---

## 🖼️ Next Image

```tsx
import Image from 'next/image';

// Full width/height
<Image
  src={imageUrl}
  alt="description"
  fill
  className="object-cover"
/>

// Fixed size
<Image
  src={imageUrl}
  alt="description"
  width={500}
  height={500}
/>
```

---

## 🔔 Toast Notifications

```typescript
import toast from 'react-hot-toast';

// نجاح
toast.success('تم بنجاح!');

// خطأ
toast.error('حدث خطأ!');

// معلومة
toast('رسالة عادية');

// تحميل
const id = toast.loading('جاري التحميل...');
// ...
toast.dismiss(id);
toast.success('تم!');
```

---

## 🧭 Navigation

```tsx
import { useRouter } from 'next/navigation';
import Link from 'next/link';

// Link
<Link href="/products">المنتجات</Link>

// Programmatic navigation
const router = useRouter();
router.push('/cart');
router.back();
router.refresh();
```

---

## 📝 Forms

```tsx
const [formData, setFormData] = useState({
  name: '',
  email: '',
});

const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
  setFormData({ ...formData, [e.target.name]: e.target.value });
};

const handleSubmit = async (e: React.FormEvent) => {
  e.preventDefault();
  // submit logic
};

<form onSubmit={handleSubmit}>
  <input
    name="name"
    value={formData.name}
    onChange={handleChange}
    required
  />
</form>
```

---

## 🔒 Protected Routes

```tsx
'use client';

import { useAuth } from '@/contexts/AuthContext';
import { useRouter } from 'next/navigation';
import { useEffect } from 'react';

export default function ProtectedPage() {
  const { user } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (!user) {
      router.push('/login');
    }
  }, [user]);

  if (!user) return null;

  return <div>محمي</div>;
}
```

---

## 🎯 TypeScript Types

```typescript
// User
interface User {
  id: string;
  email: string;
  displayName: string;
  phoneNumber?: string;
  imageUrl?: string;
  roles: string[];
}

// Product
interface Product {
  id: number;
  name: string;
  price: number;
  discountedPrice?: number;
  imageUrl: string;
  stock: number;
  isFavorite?: boolean;
}

// Cart Item
interface CartItem {
  id: number;
  productId: number;
  productName: string;
  productImage: string;
  quantity: number;
  price: number;
  totalPrice: number;
}
```

---

## 🌐 Environment Variables

```env
# .env.local
NEXT_PUBLIC_API_URL=http://localhost:5000/api
```

```tsx
// استخدام
const apiUrl = process.env.NEXT_PUBLIC_API_URL;
```

---

## 📱 Responsive Design

```tsx
// Mobile-first approach
<div className="
  w-full           // Mobile: full width
  md:w-1/2         // Tablet: half width
  lg:w-1/3         // Desktop: third width
  xl:w-1/4         // Large: quarter width
">
</div>

// Breakpoints
// sm: 640px
// md: 768px
// lg: 1024px
// xl: 1280px
// 2xl: 1536px
```

---

## 🔍 الحصول على البيانات

```tsx
'use client';

import { useEffect, useState } from 'react';
import api from '@/lib/api';

export default function MyPage() {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      const response = await api.get('/endpoint');
      setData(response.data.data);
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <div>جاري التحميل...</div>;

  return <div>{/* عرض البيانات */}</div>;
}
```

---

## 🎨 Loading States

```tsx
// Skeleton
<div className="animate-pulse">
  <div className="h-4 bg-gray-200 rounded w-3/4 mb-2"></div>
  <div className="h-4 bg-gray-200 rounded w-1/2"></div>
</div>

// Spinner
<div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
```

---

## 📅 Date Formatting

```typescript
import { format } from 'date-fns';
import { ar } from 'date-fns/locale';

// تنسيق التاريخ
const formatted = format(new Date(), 'PPP', { locale: ar });
// "١٦ أكتوبر ٢٠٢٤"

const formatted2 = format(new Date(), 'PPp', { locale: ar });
// "١٦ أكتوبر ٢٠٢٤ في ٣:٣٠ م"
```

---

## 🔗 Useful Links

- API Base: `http://localhost:5000/api`
- Dev Server: `http://localhost:3000`
- [Next.js Docs](https://nextjs.org/docs)
- [Tailwind Docs](https://tailwindcss.com/docs)

---

## 🆘 مشاكل شائعة

```bash
# Port مستخدم
# ويندوز
netstat -ano | findstr :3000
taskkill /PID <PID> /F

# ماك/لينكس
lsof -ti:3000 | xargs kill -9

# حذف Cache
rm -rf .next

# إعادة تثبيت
rm -rf node_modules package-lock.json
npm install
```

---

## 📦 إضافة حزمة جديدة

```bash
# Production
npm install package-name

# Development
npm install --save-dev package-name

# حذف
npm uninstall package-name
```

---

**احفظ هذا الملف للرجوع إليه سريعاً! ⚡**

