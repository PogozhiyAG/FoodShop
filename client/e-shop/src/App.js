import React from 'react';
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { AuthProvider } from './context/AuthContext';
import Category from './Category';
import Product from './Product';
import Home from './Home';
import NoPage from './NoPage';
import Basket from './Basket'


const App = () => {
  return (    
    
      <AuthProvider>
        <BrowserRouter>
          <Routes>
          <Route index element={<Home />} />
              <Route path="category/*" element={<Category />} />
              <Route path="product/*" element={<Product />} />
              <Route path="basket" element={<Basket />} />
              <Route path="*" element={<NoPage />} />
          </Routes>
        </BrowserRouter>
      </AuthProvider>
    
  );
}

export default App;
