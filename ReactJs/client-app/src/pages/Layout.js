import { Outlet, Link } from "react-router-dom";

const Layout = () => {
  return (
    <>
      {/* <nav>
        <ul>
          <li>
            <Link to="/">Home</Link>
          </li>
          <li>
            <Link to="/blogs">Blogs</Link>
          </li>
          <li>
            <Link to="/contact">Contact</Link>
          </li>
          <li>
            <Link to="/person">Person</Link>
          </li>
          <li>
            <Link to="/daily">Đại lý</Link>
          </li>
        </ul>
      </nav> */}

      <nav class="navbar navbar-expand-lg navbar-light bg-light">
        <a class="navbar-brand" href="#">ReactJS demo</a>
        <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarText" aria-controls="navbarText" aria-expanded="false" aria-label="Toggle navigation">
          <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarText">
          <ul class="navbar-nav mr-auto">
            <li class="nav-item active">
            <a class="nav-link" href="/">Home</a>
            </li>
            <li class="nav-item">
            <a class="nav-link" href="/blogs">Blogs</a>
            </li>
            <li class="nav-item">
            <a class="nav-link" href="/contact">Contact</a>
            </li>
            <li class="nav-item">
            <a class="nav-link" href="/person">Person</a>
            </li>
            <li class="nav-item">
            {/* <Link to="/employee">Employee</Link> */}
            <a class="nav-link" href="/daily">Đại lý</a>
            </li>
          </ul>
          
        </div>
      </nav>
      <Outlet />
    </>
  )
};

export default Layout;