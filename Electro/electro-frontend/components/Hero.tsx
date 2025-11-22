import Link from 'next/link';

export default function Hero() {
  return (
    <div className="relative bg-gradient-to-r from-blue-600 to-blue-800 text-white">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-20 md:py-32">
        <div className="text-center">
          <h1 className="text-4xl md:text-6xl font-bold mb-6 text-white drop-shadow-lg">
            مرحباً بك في Electro Medical
          </h1>
          <p className="text-xl md:text-2xl mb-8 text-white font-medium drop-shadow">
            أحدث الأجهزة والمعدات الطبية بأفضل الأسعار والجودة
          </p>
          <p className="text-lg md:text-xl mb-8 text-white font-normal drop-shadow">
            أسرة طبية • أجهزة قياس الضغط • أجهزة السكر • كراسي متحركة • مستلزمات طبية
          </p>
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <Link
              href="/products"
              className="bg-white text-blue-700 px-8 py-3 rounded-lg font-bold hover:bg-blue-50 transition shadow-lg"
            >
              تصفح المنتجات الطبية
            </Link>
            <Link
              href="/categories"
              className="border-2 border-white bg-transparent text-white px-8 py-3 rounded-lg font-bold hover:bg-white hover:text-blue-700 transition shadow-lg"
            >
              الفئات الطبية
            </Link>
          </div>
        </div>
      </div>
      
      {/* شكل زخرفي */}
      <div className="absolute bottom-0 left-0 right-0">
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1440 320">
          <path
            fill="#f9fafb"
            fillOpacity="1"
            d="M0,96L48,112C96,128,192,160,288,160C384,160,480,128,576,122.7C672,117,768,139,864,138.7C960,139,1056,117,1152,117.3C1248,117,1344,139,1392,149.3L1440,160L1440,320L1392,320C1344,320,1248,320,1152,320C1056,320,960,320,864,320C768,320,672,320,576,320C480,320,384,320,288,320C192,320,96,320,48,320L0,320Z"
          ></path>
        </svg>
      </div>
    </div>
  );
}

