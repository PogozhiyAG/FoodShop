import { useEffect, useState } from "react";

const Home = () => {
  const [products, setProducts] = useState([]);

  useEffect(() => {
    fetch('https://localhost:62788/Catalog')
    .then(r => r.json())
    .then(r => setProducts(r))
  }, []);


  return (
    <>
      <h1>Home</h1>
      {products.map(i => <div>{i.name}</div>)}
    </>
  );
};

export default Home;