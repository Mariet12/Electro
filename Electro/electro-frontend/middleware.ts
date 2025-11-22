import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

export function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;

  // Protected routes
  const protectedRoutes = ['/profile', '/cart', '/checkout', '/orders', '/favorites', '/notifications'];
  const adminRoutes = ['/admin'];

  // Check if route is protected
  const isProtectedRoute = protectedRoutes.some(route => pathname.startsWith(route));
  const isAdminRoute = adminRoutes.some(route => pathname.startsWith(route));

  // ملاحظة: لا يمكن الوصول لـ localStorage في middleware (server-side)
  // لذلك سنعتمد على client-side protection في الصفحات نفسها
  // الـ middleware هنا فقط للتحقق من cookies (إذا كان موجود)
  
  // إذا كان هناك token في cookies، اتركه يمر
  // وإلا، سيتم التحقق في client-side
  const token = request.cookies.get('token')?.value;
  
  // لا نعيد التوجيه هنا - نترك الصفحة تتحقق من localStorage في client-side
  // هذا أفضل لأن localStorage لا يمكن الوصول إليه في server-side middleware

  return NextResponse.next();
}

export const config = {
  matcher: [
    '/profile/:path*',
    '/cart/:path*',
    '/checkout/:path*',
    '/orders/:path*',
    '/favorites/:path*',
    '/notifications/:path*',
    '/admin/:path*',
  ],
};

