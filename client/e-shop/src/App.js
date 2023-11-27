import React from 'react';
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { FoodShopProvider } from './context/FoodShopContext';
import Category from './Category';
import Home from './Home';
import NoPage from './NoPage';
import Basket from './Basket';
import './App.css';


const App = () => {
  return (
      <FoodShopProvider>
        <BrowserRouter>
          <Routes>
          <Route index element={<Home />} />
              <Route path="category/*" element={<Category />} />              
              <Route path="basket" element={<Basket />} />
              <Route path="*" element={<NoPage />} />
          </Routes>
        </BrowserRouter>
      </FoodShopProvider>
  );
}

export default App;
