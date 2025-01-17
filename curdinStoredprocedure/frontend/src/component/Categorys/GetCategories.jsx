import React, { useEffect, useState } from "react";
import axios from "axios";
import GetProductItemsByCategory from "./GetProductItemsByCategory"; 
import ProductItems from "../ProductItems/ProductItems";
import "./CSS/GetCategories.css"; 

const GetCategories = () => {
    const [categories, setCategories] = useState([]);
    const [error, setError] = useState("");
    const [selectedCategoryId, setSelectedCategoryId] = useState(null); 
    const [selectedCategoryName, setSelectedCategoryName] = useState(null); 

    const fetchCategories = async () => {
        try {
            const response = await axios.get("https://localhost:7217/api/productcategories");
            setCategories(response.data);
        } catch (err) {
            setError("Failed to fetch categories.");
        }
    };

    useEffect(() => {
        fetchCategories();
    }, []);

    return (
        <div className="categories-container mb-4">
            <h3>Product Categories</h3>
            {error && <div className="alert alert-danger">{error}</div>}
            <div className="btn-group">
                <button
                    className="btn-clear"
                    onClick={() => setSelectedCategoryId(null)}
                >
                    Get All Products
                </button>
                {categories.map((category) => (
                    <button
                        key={category.categoryId}
                        className="btn-category"
                        onClick={() => {
                            setSelectedCategoryId(category.categoryId);
                            setSelectedCategoryName(category.categoryName);
                        }}
                    >
                        {category.categoryName}
                    </button>
                ))}
            </div>

            {selectedCategoryId ? (
                <GetProductItemsByCategory categoryId={selectedCategoryId} categoryName={selectedCategoryName} />
            ) : (
                <ProductItems />
            )}
        </div>
    );
};

export default GetCategories;
