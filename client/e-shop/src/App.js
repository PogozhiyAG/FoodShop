import {useState, useEffect} from 'react';
import logo from './logo.svg';
import './App.css';

function App() {
  const[data, setData] = useState([]);
  const[categoryId, setCategoryId] = useState(15);

  useEffect(() => {
    fetch(`https://localhost:51975/Catalog?categoryId=${categoryId}&take=15`)
    .then(r => r.json())
    .then(r => setData(r));
  }, [categoryId]);

  return (
    <div>
      <header>
        <h1>E-Shop</h1>
      </header>
      <input value={categoryId} onChange={(e) => setCategoryId(e.target.value)}></input>
      <div>
        {data.map(p => <div key={p.id}>{p.name}</div>)}
      </div>
    </div>
  );
}

export default App;
