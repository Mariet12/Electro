import type { Metadata } from "next";
import { Inter, Cairo } from "next/font/google";
import "./globals.css";
import { AuthProvider } from "@/contexts/AuthContext";
import { CartProvider } from "@/contexts/CartContext";
import { Toaster } from "react-hot-toast";

const cairo = Cairo({ 
  subsets: ["arabic"],
  weight: ['300', '400', '500', '600', '700'],
  variable: '--font-cairo',
});

export const metadata: Metadata = {
  title: "Electro Medical - متجر الأجهزة الطبية",
  description: "أفضل متجر للأجهزة والمعدات الطبية - أسرة طبية، أجهزة قياس الضغط والسكر، كراسي متحركة، ومستلزمات طبية",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="ar" dir="rtl">
      <body className={cairo.className}>
        <AuthProvider>
          <CartProvider>
            {children}
            <Toaster 
              position="top-center"
              reverseOrder={false}
              toastOptions={{
                duration: 3000,
                style: {
                  fontFamily: 'Cairo, sans-serif',
                },
              }}
            />
          </CartProvider>
        </AuthProvider>
      </body>
    </html>
  );
}

