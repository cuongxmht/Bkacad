import logo from './logo.svg';
import ReactDOM from "react-dom/client";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import './App.css';
import {useState} from 'react';
import Home from './pages/Home'
import Layout from './pages/Layout'
import Blogs from "./pages/Blogs";
import Contact from "./pages/Contact";
import NoPage from "./pages/NoPage";

import Person from './pages/Person'
import Employee from './pages/Employee'
// function App() {
//   return (
//     <div className="App">
//       <header className="App-header">
//         <img src={logo} className="App-logo" alt="logo" />
//         <p>
//           Edit <code>src/App.js</code> and save to reload.
//         </p>
//         <a
//           className="App-link"
//           href="https://reactjs.org"
//           target="_blank"
//           rel="noopener noreferrer"
//         >
//           Learn React
//         </a>
//       </header>
//     </div>
//   );
// }

// export default App;

function MyButton() {
  let rand;
  const [count, setCount]=useState(0);
  function handleClick() {
    rand=Math.random();
    setCount(count+1);
  }

  return (
    <button onClick={handleClick}>
      Click {count} times
    </button>
  );
}
function AboutPage() { 
  return (
    <>
          <h1>About</h1>
          <p>Hello there.<br />How do you do?</p>   
    </>
  );
}

// export default function MyApp() {
//   return (
//     <div>
//       <h1>Welcome to my app</h1>
//       <MyButton />
//     </div>
//   );
// }

const products = [
  { title: 'Cabbage', id: 1 },
  { title: 'Garlic', id: 2 },
  { title: 'Apple', id: 3 },
];

const user = {
  name: 'Hedy Lamarr',
  imageUrl: 'https://i.imgur.com/yXOvdOSs.jpg',
  imageSize: 90,
};

const listItems = products.map(product =>
  <li key={product.id}>
    {product.title}
  </li>
);
/*
export default function Profile() {
  let isAuthen=false;
  let contents;
  if (isAuthen)
        contents = <AboutPage />
      else contents = (
        <>
          <MyButton/>
          <MyButton/>
        </>
      )
  return (
    <>
      <h1>{user.name}</h1>
      <img
        className="avatar"
        src={user.imageUrl}
        alt={'Photo of ' + user.name}
        style={{
          width: user.imageSize,
          height: user.imageSize
        }}
      />
      
      {contents}
      <ul>
        {listItems}
      </ul>
    </>
  );
}*/

export default function App(){
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route index element={<Home />} />
          <Route path="blogs" element={<Blogs />} />
          <Route path="contact" element={<Contact />} />
          <Route path='person' element={<Person/>} />
          <Route path='employee' element={<Employee/>}/>
          <Route path="*" element={<NoPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

