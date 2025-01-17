import React , { useState } from "react";

import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import NavBar from "./component/NavBar/NavBar";
import ProductItems from "./component/ProductItems/ProductItems";
import CreateProductCategory from "./component/Categorys/createCategory";
import GetCategories from "./component/Categorys/GetCategories";
import UpdateCategory from "./component/Categorys/UpdateCategory";
import DeleteCategory from "./component/Categorys/DeleteCategory";
import CreateProductItem from "./component/ProductItems/CreateProductItem";
import UpdateProductItem from "./component/ProductItems/UpdateProductItem";
import Signup from "./component/UserComponent/SignUp";
import SignIn from "./component/UserComponent/SignIn";
import AuthCallback from './component/UserComponent/AuthCallback';
import ProductedRoute from "../src/component/ProductedRoute";
import Addtocart from "../src/component/Categorys/Cart/Addtocart";
import Admindashboard from "../src/component/Admin/Admindashboard";
import ChartandGraph from "../src/component/Admin/ChartandGraph";
function App() {
    const userName = localStorage.getItem('userName');

    const [isLogin, setisLogin] = useState(false);
    const [UserName, setUserName] = useState(userName);
    console.log(UserName);
    return (
        <Router>
            <div className="App">
                <NavBar isLogin={isLogin}
                    setisLogin={setisLogin}
                    UserName={UserName} setUserName={setUserName}              />
                <Routes>
                    <Route element={<ProductedRoute />}>
                    <Route path="/" element={<GetCategories />} />
                    <Route path="/categories/create" element={<CreateProductCategory />} />
                    <Route path="/categories/update/:id" element={<UpdateCategory />} />
                    <Route path="/update-product/:productId" element={<UpdateProductItem />} />
                    <Route path="/categories/delete/:id" element={<DeleteCategory />} />
                    <Route path="/products" element={<ProductItems />} />
                        <Route path="/products/create" element={<CreateProductItem />} />
                        <Route path="/add-to-cart" element={<Addtocart />} />
                        <Route path="/adminboard" element={<Admindashboard />} />
                        <Route path="/chartandGraph" element={<ChartandGraph />} />
                    </Route>
                    <Route path="/auth/callback" element={<AuthCallback setisLogin={setisLogin} setUserName={setUserName} />} />

                    <Route path="/signup" element={<Signup />} />
                    <Route path="/SignIn" element={<SignIn setisLogin={setisLogin} setUserName={setUserName} />} />
                </Routes>
            </div>
        </Router>
    );
}

export default App;
