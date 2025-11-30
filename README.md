# Electro - Electronics Store

Modern and comprehensive frontend for an electronics e-commerce store built with Next.js 14, TypeScript, and Tailwind CSS.

## 🚀 Features

### For Users

✅ Complete login and registration system  
✅ Password reset via OTP  
✅ Product browsing with advanced filters  
✅ Product search  
✅ Dynamic shopping cart  
✅ Complete order system  
✅ Order status tracking  
✅ Favorite products  
✅ Notifications  
✅ Profile page  
✅ Contact us page  

### For Administrators

✅ Admin dashboard  
✅ Order management and status updates  
✅ Sales statistics  
✅ Payment status management  

## 📦 Technologies Used

- **Next.js 14** - App Router
- **TypeScript** - Type-safe development
- **Tailwind CSS** - Styling
- **Axios** - API communication
- **React Context** - State management
- **React Hot Toast** - Notifications
- **React Icons** - Icons
- **date-fns** - Date formatting

## 🛠️ Installation & Setup

### Prerequisites

- Node.js 18+
- npm, yarn, or pnpm

### Installation Steps

1. **Install dependencies:**
```bash
cd electro-frontend
npm install
```

2. **Environment setup:** Create a `.env.local` file in the root directory:
```env
NEXT_PUBLIC_API_URL=http://localhost:5000/api
```

3. **Run the project:**

**Development mode:**
```bash
npm run dev
```

**Production build:**
```bash
npm run build
npm start
```

The project will run on: [http://localhost:3000](http://localhost:3000)

## 📁 Project Structure

```
electro-frontend/
├── app/                      # Next.js pages (App Router)
│   ├── admin/               # Admin pages
│   ├── cart/                # Cart page
│   ├── categories/          # Categories page
│   ├── checkout/            # Checkout page
│   ├── contact/             # Contact page
│   ├── favorites/           # Favorites page
│   ├── forgot-password/     # Forgot password
│   ├── login/               # Login
│   ├── notifications/       # Notifications
│   ├── orders/              # Orders
│   ├── products/            # Products
│   ├── profile/             # Profile
│   ├── register/            # Registration
│   ├── reset-password/      # Reset password
│   ├── verify-otp/          # OTP verification
│   ├── layout.tsx           # Main layout
│   └── page.tsx             # Home page
├── components/              # Reusable components
│   ├── Navbar.tsx
│   ├── Footer.tsx
│   ├── Hero.tsx
│   ├── ProductCard.tsx
│   ├── LatestProducts.tsx
│   ├── BestSelling.tsx
│   └── Categories.tsx
├── contexts/                # Context Providers
│   ├── AuthContext.tsx
│   └── CartContext.tsx
├── lib/                     # Utilities
│   └── api.ts              # Axios instance
├── public/                  # Static files
├── .env.local              # Environment variables
├── next.config.js          # Next.js configuration
├── tailwind.config.ts      # Tailwind configuration
└── package.json
```

## 🔌 API Integration

The project integrates with the Electro Backend API (.NET)

### Used Endpoints:

**Authentication:**
- `POST /api/account/register` - Create account
- `POST /api/account/login` - Login
- `GET /api/account/user-info` - User data
- `PUT /api/account/update-user` - Update profile
- `PUT /api/account/change-password` - Change password
- `POST /api/account/forgot-password` - Forgot password
- `POST /api/account/verify-otp` - Verify OTP
- `PUT /api/account/reset-password` - Reset password

**Products:**
- `GET /api/products` - Product list with filters
- `GET /api/products/{id}` - Product details
- `GET /api/products/latest` - Latest products
- `GET /api/products/best-selling` - Best selling products

**Categories:**
- `GET /api/category` - Category list

**Cart:**
- `GET /api/cart` - View cart
- `POST /api/cart/add` - Add to cart
- `PUT /api/cart/items` - Update quantity
- `DELETE /api/cart/items/{id}` - Remove from cart
- `DELETE /api/cart` - Clear cart

**Orders:**
- `POST /api/orders/checkout` - Create order
- `GET /api/orders` - User orders
- `GET /api/orders/{id}` - Order details
- `PUT /api/orders/{id}/cancel` - Cancel order
- `GET /api/orders/admin/all` - All orders (Admin)
- `PUT /api/orders/{id}/status` - Update order status (Admin)

**Favorites:**
- `GET /api/favorites` - Favorites list
- `POST /api/favorites` - Add to favorites
- `DELETE /api/favorites/{id}` - Remove from favorites

**Notifications:**
- `GET /api/notifications` - Notifications list
- `PUT /api/notifications/{id}/read` - Mark as read

**Contact:**
- `POST /api/admincontact` - Send message

## 🎨 Design

The design is built on:
- **Color System:** Primary (blue), with dark mode support
- **Typography:** Cairo font for Arabic
- **Responsive Design:** Responsive across all screen sizes
- **RTL Support:** Full Arabic support

## 🔒 Security

✅ JWT Authentication  
✅ Axios Interceptors for token handling  
✅ Protected Routes  
✅ Input Validation  
✅ HTTPS in production  

## 📱 Pages

### Public
- `/` - Home page
- `/products` - Product list
- `/products/[id]` - Product details
- `/categories` - Categories
- `/contact` - Contact

### User
- `/login` - Login
- `/register` - Create account
- `/forgot-password` - Forgot password
- `/verify-otp` - OTP verification
- `/reset-password` - Reset password
- `/profile` - Profile
- `/cart` - Cart
- `/checkout` - Checkout
- `/orders` - Orders
- `/orders/[id]` - Order details
- `/favorites` - Favorites
- `/notifications` - Notifications

### Administration (Admin)
- `/admin` - Dashboard
- `/admin/orders` - Order management

## 🚀 Deployment

### Vercel (Recommended)

```bash
npm run build
vercel deploy
```

### Docker

```bash
docker build -t electro-frontend .
docker run -p 3000:3000 electro-frontend
```

## 🤝 Contributing

Contributions are welcome! Please:

1. Fork the project
2. Create a new branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License

## 📞 Support

For support and assistance:

- **Email:** marietayman1@gmail.com
- **Phone:** 01206799037

---

Built with ❤️ using Next.js and TypeScript
