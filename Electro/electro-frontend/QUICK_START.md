# 🚀 دليل البدء السريع - Electro Frontend

## ⚡ التثبيت في 3 خطوات

```bash
# 1. تثبيت الحزم
npm install

# 2. إنشاء ملف البيئة
echo "NEXT_PUBLIC_API_URL=http://localhost:5000/api" > .env.local

# 3. تشغيل المشروع
npm run dev
```

افتح المتصفح على: http://localhost:3000

## 📁 الصفحات الرئيسية

| المسار | الوصف | الوصول |
|--------|-------|--------|
| `/` | الصفحة الرئيسية | عام |
| `/login` | تسجيل الدخول | عام |
| `/register` | إنشاء حساب | عام |
| `/products` | قائمة المنتجات | عام |
| `/products/[id]` | تفاصيل منتج | عام |
| `/cart` | السلة | مستخدم |
| `/checkout` | إتمام الطلب | مستخدم |
| `/orders` | الطلبات | مستخدم |
| `/profile` | الملف الشخصي | مستخدم |
| `/admin` | لوحة التحكم | مدير |

## 🔑 المصادقة

### تسجيل حساب جديد
```typescript
POST /api/account/register
{
  "email": "user@example.com",
  "password": "Password123!",
  "displayName": "اسم المستخدم",
  "phoneNumber": "01234567890"
}
```

### تسجيل الدخول
```typescript
POST /api/account/login
{
  "email": "user@example.com",
  "password": "Password123!"
}
```

## 🛒 استخدام Cart Context

```typescript
import { useCart } from '@/contexts/CartContext';

function MyComponent() {
  const { cart, addToCart, removeItem } = useCart();
  
  // إضافة منتج
  await addToCart(productId, quantity);
  
  // حذف منتج
  await removeItem(cartItemId);
  
  return <div>عدد المنتجات: {cart?.itemsCount}</div>;
}
```

## 👤 استخدام Auth Context

```typescript
import { useAuth } from '@/contexts/AuthContext';

function MyComponent() {
  const { user, login, logout } = useAuth();
  
  if (!user) return <div>الرجاء تسجيل الدخول</div>;
  
  return (
    <div>
      <p>مرحباً {user.displayName}</p>
      <button onClick={logout}>تسجيل الخروج</button>
    </div>
  );
}
```

## 🎨 استخدام Tailwind Classes

```tsx
// الألوان الأساسية
className="text-primary-600 bg-primary-50"

// الأزرار
className="bg-primary-600 text-white px-4 py-2 rounded-lg hover:bg-primary-700"

// البطاقات
className="bg-white rounded-lg shadow-md p-6"

// Grid
className="grid grid-cols-1 md:grid-cols-3 gap-6"
```

## 📡 استدعاء API

```typescript
import api from '@/lib/api';

// GET
const response = await api.get('/products');
const products = response.data.data;

// POST
const response = await api.post('/cart/add', {
  productId: 1,
  quantity: 2
});

// PUT
const response = await api.put('/account/update-user', formData);

// DELETE
await api.delete('/cart/items/1');
```

## 🔔 Toast Notifications

```typescript
import toast from 'react-hot-toast';

// نجاح
toast.success('تم بنجاح!');

// خطأ
toast.error('حدث خطأ!');

// تحميل
const toastId = toast.loading('جاري التحميل...');
toast.dismiss(toastId);
```

## 🖼️ استخدام الصور

```typescript
import Image from 'next/image';

<Image
  src={product.imageUrl || '/placeholder.png'}
  alt={product.name}
  fill
  className="object-cover"
/>
```

## 🔐 الحماية

الصفحات المحمية تلقائياً:
- `/cart`
- `/checkout`
- `/orders`
- `/profile`
- `/favorites`
- `/notifications`
- `/admin/*`

## 🎯 المكونات الجاهزة

```typescript
// Navbar
import Navbar from '@/components/Navbar';
<Navbar />

// Footer
import Footer from '@/components/Footer';
<Footer />

// Product Card
import ProductCard from '@/components/ProductCard';
<ProductCard product={product} />
```

## 🧪 الأوامر المفيدة

```bash
# التطوير
npm run dev

# البناء
npm run build

# التشغيل (production)
npm start

# Lint
npm run lint

# Type check
npx tsc --noEmit
```

## 📝 البيانات الوهمية للتجربة

### حساب مستخدم عادي
```
Email: user@test.com
Password: Test123!
```

### حساب مدير
```
Email: admin@test.com
Password: Admin123!
```

## 🐛 تصحيح الأخطاء

### تفعيل Console Logs

في `lib/api.ts`:
```typescript
api.interceptors.request.use((config) => {
  console.log('Request:', config);
  return config;
});
```

### عرض بيانات الـ Context

```typescript
const { user } = useAuth();
console.log('Current user:', user);

const { cart } = useCart();
console.log('Current cart:', cart);
```

## 🔄 تحديث البيانات

```typescript
// تحديث بيانات المستخدم
const { updateUser } = useAuth();
await updateUser();

// تحديث السلة
const { refreshCart } = useCart();
await refreshCart();
```

## 📱 Responsive Design

```typescript
// Mobile: default
// Tablet: md:
// Desktop: lg:
// Large: xl:

<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4">
  {/* محتوى */}
</div>
```

## 🌐 الترجمة

المشروع يدعم العربية افتراضياً:
- RTL Layout
- خط Cairo
- Date-fns locale: ar

## ⚡ نصائح للأداء

```typescript
// استخدم dynamic import للمكونات الثقيلة
const HeavyComponent = dynamic(() => import('./HeavyComponent'), {
  loading: () => <div>جاري التحميل...</div>
});

// Image optimization
<Image
  src={src}
  alt={alt}
  width={500}
  height={500}
  placeholder="blur"
/>
```

---

للمزيد من التفاصيل، راجع: [README.md](./README.md) أو [INSTALLATION.md](./INSTALLATION.md)

