import React, { useState, useEffect } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom"; // Import useNavigate
import "./CSS/CreaateProductItmex.css"
const CreateProductItem = () => {
    const [productName, setProductName] = useState("");
    const [price, setPrice] = useState("");
    const [categoryId, setCategoryId] = useState("");
    const [categories, setCategories] = useState([]);
    const [description, setDescription] = useState("");
    const [error, setError] = useState("");
    const navigate = useNavigate();

    // Fetch available categories
    useEffect(() => {
        const fetchCategories = async () => {
            try {
                const response = await axios.get("https://localhost:7217/api/productcategories")
                setCategories(response.data);
                console.log(response.data)
            } catch (err) {
                setError("Failed to fetch categories.");
            }
        };
        fetchCategories();
    }, []);

    const handleCreate = async () => {
        if (!productName || !price || !categoryId || !description) {
            setError("All fields are required.");
            return;
        }

        try {
            await axios.post("https://localhost:7217/api/productitems", {
                productName,
                price: parseInt(price),
                categoryId,
                description,
            });
            navigate("/")
            setProductName("")
            setPrice("")
            setCategoryId("")
            setDescription("")

        } catch (err) {
            setError("Failed to create product item.");
        }
    };

    return (
        <div className="create-product-container">
            <h3>Create Product Item</h3>
            {error && <div className="error-message">{error}</div>}
            <div className="input-group">
                <label>Product Name</label>
                <input
                    type="text"
                    value={productName}
                    onChange={(e) => setProductName(e.target.value)}
                    className="input-field"
                />
            </div>
            <div className="input-group">
                <label>Price</label>
                <input
                    type="number"
                    step="0.01"
                    value={price}
                    onChange={(e) => setPrice(e.target.value)}
                    className="input-field"
                />
            </div>
            <div className="input-group">
                <label>Category</label>
                <select
                    value={categoryId}
                    onChange={(e) => setCategoryId(e.target.value)}
                    className="input-field"
                >
                    <option value="">Select a category</option>
                    {categories.map((category) => (
                        <option key={category.categoryId} value={category.categoryId}>
                            {category.categoryName}
                        </option>
                    ))}
                </select>
            </div>
            <div className="input-group">
                <label>Description</label>
                <input
                    type="text"
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    className="input-field"
                />
            </div>
            <button className="submit-button" onClick={handleCreate}>
                Create
            </button>
        </div>
    );
};

export default CreateProductItem;
