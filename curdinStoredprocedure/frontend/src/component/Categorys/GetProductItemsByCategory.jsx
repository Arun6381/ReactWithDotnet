import React, { useState, useEffect } from "react";
import axios from "axios";
import "./CSS/GetProductByCategory.css"; // Import the updated CSS file

const ProductItemsByCategory = ({ categoryId, categoryName }) => {
    const [productItems, setProductItems] = useState([]);
    const [error, setError] = useState("");

    const fetchProductItems = async () => {
        try {
            const response = await axios.get(`https://localhost:7217/api/ProductItems/category/${categoryId}`);
            const data = response.data;
            if (!data || data.length === 0) {
                setError("No products found for the selected category.");
            } else {
                setProductItems(data);
                setError("");
            }
        } catch (err) {
            alert("No products found for the selected category.");
        }
    };

    useEffect(() => {
        fetchProductItems();
    }, [categoryId]);

    return (
        <div className="product-category-container">
            <h3 className="category-title">Products in {categoryName}</h3>
            {error && <div className="error-message">{error}</div>}
            <div className="product-list">
                {productItems.map((item) => (
                    <div key={item.id} className="product-card">
                        <div className="product-details">
                            <strong className="product-name">{item.productName}</strong>
                            <span className="product-description">{item.description}</span>
                            <div className="product-price">${item.price}</div>
                            <button className="add-to-cart-btn">Add To Cart</button>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default ProductItemsByCategory;
