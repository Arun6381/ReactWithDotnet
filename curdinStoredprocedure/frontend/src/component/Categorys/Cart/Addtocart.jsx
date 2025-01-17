import React, { useState, useEffect } from "react";
import axios from "axios";
import "./AddtoCart.css"
const CartDetails = () => {
    const [cartDetails, setCartDetails] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const userId = localStorage.getItem("UserId") ?? localStorage.getItem("userId")

    // Fetch cart details for the user
    const fetchCartDetails = async () => {
        try {
            setLoading(true);
            const response = await axios.get(`https://localhost:7217/api/AddToCart/GetCartDetailsByUserId/${userId}`);
            console.log(response.data)
            setCartDetails(response.data);
            setError(null);
        } catch (err) {
            setError("Failed to fetch cart details.");
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (cartId) => {
        console.log(cartId)
        try {
            await axios.delete(`https://localhost:7217/api/AddToCart/DeleteCartItem/${cartId}`);
            setCartDetails(cartDetails.filter((item) => item.CartId !== cartId));
            alert("Cart item deleted successfully.");
        } catch (err) {
            alert("Failed to delete cart item.");
        }
    };
    const handlePlacetheOrder = async (cartId) => {
        try {
            await axios.put(`https://localhost:7217/api/AddToCart/UpdateStatus/${cartId}`);

            fetchCartDetails();

            alert("Order placed successfully.");
        } catch (err) {
            alert("Failed to place the order.");
        }
    };


    useEffect(() => {
        if (userId) {
            fetchCartDetails();
        }

    }, []);

    if (loading) return <p>Loading cart details...</p>;
    if (error) return <p className="error-message">{"no product is added to the cart"}</p>;

    return (
        <div className="cart-details-container">
            {cartDetails.length === 0 ? (
                <p>No items in the cart.</p>
            ) : (
                <ul className="cart-list">
                    {cartDetails.map((item) => (
                        <li key={item.cartId} className="cart-item">
                            <h3>{item.productName}</h3>
                            <p>Price: ${item.price}</p>
                            <p>Status: {item.status === 1 ? "Completed" : "Pending"}</p>
                            {item.status === 1 ? <span>Order Placed Successfully</span> : <button onClick={() => handlePlacetheOrder(item.cartId)}>Place the order</button>}                            

                            <button onClick={() => handleDelete(item.cartId)}>Delete</button>
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
};

export default CartDetails;
