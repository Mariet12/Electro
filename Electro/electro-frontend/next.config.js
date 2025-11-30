/** @type {import('next').NextConfig} */
const nextConfig = {
  // Vercel لا يحتاج output: 'standalone'
  images: {
    remotePatterns: [
      {
        protocol: 'http',
        hostname: 'localhost',
        port: '5008',
        pathname: '/uploads/**',
      },
      {
        protocol: 'https',
        hostname: 'kerolosadel12-002-site1.qtempurl.com',
        pathname: '/uploads/**',
      },
      {
        protocol: 'https',
        hostname: 'firebasestorage.googleapis.com',
        pathname: '/v0/b/elctro-ed5d4.appspot.com/o/**',
      },
    ],
    unoptimized: false,
    formats: ['image/avif', 'image/webp'],
    deviceSizes: [640, 750, 828, 1080, 1200, 1920],
    imageSizes: [16, 32, 48, 64, 96, 128, 256, 384],
  },
  // تحسين الأداء
  compress: true,
  poweredByHeader: false,
  reactStrictMode: true,
  swcMinify: true,
}

module.exports = nextConfig

 