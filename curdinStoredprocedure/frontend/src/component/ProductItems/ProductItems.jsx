import React, { useState, useEffect } from "react";
import axios from "axios";
import UpdateProductItem from "./UpdateProductItem";
import DeleteProductItem from "./DeleteProductItem";
import { useNavigate } from 'react-router-dom';

import "./CSS/Products.css";

const ProductItems = () => {
    const [productItems, setProductItems] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [selectedProduct, setSelectedProduct] = useState(null);
    const [isUpdating, setIsUpdating] = useState(false);
    const navigate = useNavigate();

    const fetchProductItems = async () => {
        try {
            setLoading(true);
            const response = await axios.get("https://localhost:7217/api/productitems");
            setProductItems(response.data);
            console.log(response.data)
            setError(null);
        } catch (err) {
            setError("Failed to fetch product items.");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchProductItems();
    }, []);

    const handleUpdateComplete = () => {
        setIsUpdating(false);
        setSelectedProduct(null);
        fetchProductItems(); // Refresh the list
    };

    const handleDeleteComplete = () => {
        fetchProductItems(); // Refresh the list
    };

    const handleAddToCart = async (productId) => {
        try {
            const userId = localStorage.getItem("UserId") ?? localStorage.getItem("userId");
            if (!userId) {
                console.error("UserId not found in local storage.");
                return;
            }

            const response = await axios.post("https://localhost:7217/api/AddToCart/AddToCart", {
                UserId: userId,
                ProductId: productId,
            });

            if (response.status === 200) {
                console.log("Product added to cart successfully:", response.data);
                //navigate('/add-to-cart');
                alert("Product added to cart!");
            } else {
                console.error("Failed to add product to cart:", response.statusText);
                alert("Error adding product to cart.");
            }
        } catch (error) {
            console.error("Error adding to cart:", error);
            alert("Failed to add product to cart. Please try again.");
        }
    };


    if (loading) return <p>Loading product items...</p>;
    if (error) return <p className="error-message">{error}</p>;

    return (
        <div className="product-items-container">
            {productItems.length === 0 ? (
                <p>No product items available.</p>
            ) : (
                <ul className="product-list">
                    {productItems.map((item) => (
                        <li key={item.product_Id} className="product-item">
                            <h3 className="product-name">{item.productName}</h3>
                            <p className="product-description">{item.description}</p>
                            <p className="product-price">Price: ${item.price}</p>
                            <button
                                className="update-button"
                                onClick={() => { handleAddToCart(item.product_Id) }}
                            >
                                Add To Cart
                            </button>
                            <button
                                className="update-button"
                                onClick={() => {
                                    setSelectedProduct(item);
                                    setIsUpdating(true);
                                }}
                            >
                                Update
                            </button>
                            <DeleteProductItem productId={item.product_Id} onDeleteComplete={handleDeleteComplete} />
                        </li>
                    ))}
                </ul>
            )}

            {selectedProduct && isUpdating && (
                <div className="modal" style={{ display: isUpdating ? "block" : "none" }}>
                    <div className={`modal-content ${isUpdating ? "open" : ""}`}>
                        <UpdateProductItem product={selectedProduct} onUpdateComplete={handleUpdateComplete} />
                        <button
                            className="close-button"
                            onClick={() => {
                                setIsUpdating(false);
                                setSelectedProduct(null);
                            }}
                        >
                            Close
                        </button>
                    </div>
                </div>
            )}
        </div>
    );
};

export default ProductItems;
