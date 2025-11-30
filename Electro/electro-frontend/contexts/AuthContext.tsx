'use client';

import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import api from '@/lib/api';
import { useRouter } from 'next/navigation';

interface User {
  id: string;
  email: string;
  displayName: string;
  phoneNumber?: string;
  imageUrl?: string;
  roles?: string[];
  role?: string; // دعم role مفرد أيضاً
}

interface AuthContextType {
  user: User | null;
  loading: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (data: RegisterData) => Promise<void>;
  logout: () => void;
  updateUser: () => Promise<void>;
}

interface RegisterData {
  email: string;
  password: string;
  displayName: string;
  phoneNumber: string;
  role?: string;
  image?: File | null;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const router = useRouter();

  useEffect(() => {
    checkAuth();
  }, []);

  const checkAuth = async () => {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const response = await api.get('/account/user-info');
        const userData = response.data.data;
        // Ensure roles is an array
        const user = {
          ...userData,
          roles: userData.roles || (userData.role ? [userData.role] : [])
        };
        setUser(user);
      } catch (error) {
        localStorage.removeItem('token');
      }
    }
    setLoading(false);
  };

  const login = async (email: string, password: string) => {
    const response = await api.post('/account/login', { email, password });
    const { token, ...userData } = response.data.data;
    
    // حفظ التوكن في localStorage
    localStorage.setItem('token', token);
    
    // حفظ التوكن في cookies أيضاً (للمستقبل إذا احتجنا server-side access)
    document.cookie = `token=${token}; path=/; max-age=${60 * 60 * 24 * 10}; SameSite=Lax`;
    
    // Ensure roles is an array
    const user = {
      ...userData,
      roles: userData.roles || (userData.role ? [userData.role] : [])
    };
    setUser(user);
    router.push('/');
  };

  const register = async (data: RegisterData) => {
    const formData = new FormData();
    formData.append('UserName', data.displayName);
    formData.append('Email', data.email);
    formData.append('Password', data.password);
    formData.append('Role', data.role || 'User');
    if (data.phoneNumber) {
      formData.append('PhoneNumber', data.phoneNumber);
    }
    if (data.image) {
      formData.append('Image', data.image);
    }
    
    try {
      // عند إرسال FormData، axios يضيف Content-Type تلقائياً مع boundary
      // الـinterceptor في api.ts يتعامل مع هذا تلقائياً
      const response = await api.post('/account/register', formData);
      
      // بعد نجاح التسجيل نقوم بتسجيل الدخول تلقائياً للحصول على التوكن
      await login(data.email, data.password);
    } catch (error: any) {
      // معالجة أفضل للأخطاء
      console.error('Registration error:', error);
      const errorMessage = error.response?.data?.message || error.message || 'فشل إنشاء الحساب';
      const errors = error.response?.data?.errors;
      
      if (errors && Array.isArray(errors) && errors.length > 0) {
        throw new Error(errors.join(', '));
      }
      
      throw new Error(errorMessage);
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    // حذف التوكن من cookies أيضاً
    document.cookie = 'token=; path=/; max-age=0';
    setUser(null);
    router.push('/login');
  };

  const updateUser = async () => {
    const response = await api.get('/account/user-info');
    const userData = response.data.data;
    // Ensure roles is an array
    const user = {
      ...userData,
      roles: userData.roles || (userData.role ? [userData.role] : [])
    };
    setUser(user);
  };

  return (
    <AuthContext.Provider value={{ user, loading, login, register, logout, updateUser }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

