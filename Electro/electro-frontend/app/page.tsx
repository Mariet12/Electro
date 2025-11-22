import Navbar from "@/components/Navbar";
import Footer from "@/components/Footer";
import Hero from "@/components/Hero";
import LatestProducts from "@/components/LatestProducts";
import BestSelling from "@/components/BestSelling";
import Categories from "@/components/Categories";

export default function Home() {
  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />
      <Hero />
      <Categories />
      <LatestProducts />
      <BestSelling />
      <Footer />
    </div>
  );
}

