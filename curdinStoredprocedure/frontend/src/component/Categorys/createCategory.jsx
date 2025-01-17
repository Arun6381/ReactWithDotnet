import React, { useState } from "react";
import axios from "axios";
import "./CSS/createCotegory.css"
const CreateProductCategory = () => {
    const [categoryName, setCategoryName] = useState("");
    const [description, setDescription] = useState("");
    const [message, setMessage] = useState(null);

    const handleSubmit = async (e) => {
        e.preventDefault();

        const newCategory = {
            categoryName: categoryName,
            description: description,
        };

        try {
            const response = await axios.post(
                "https://localhost:7217/api/productcategories",
                newCategory
            );
            setMessage(`Category created successfully with ID: ${response.data.categoryName}`);
            setCategoryName("");
            setDescription("");
        } catch (error) {
            setMessage(
                error.response?.data?.message || "An error occurred. Please try again."
            );
        }
    };

    return (
        <div className="create-category-container">
            <h2>Create Product Category</h2>
            {message && <p className="message">{message}</p>}
            <form onSubmit={handleSubmit} className="form-container">
                <div className="form-group">
                    <label htmlFor="categoryName">Category Name:</label>
                    <input
                        type="text"
                        id="categoryName"
                        value={categoryName}
                        onChange={(e) => setCategoryName(e.target.value)}
                        required
                        className="input-field"
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="description">Description:</label>
                    <textarea
                        id="description"
                        value={description}
                        onChange={(e) => setDescription(e.target.value)}
                        required
                        className="input-field"
                    />
                </div>
                <button type="submit" className="submit-button">
                    Create Category
                </button>
            </form>
        </div>
    );
};

export default CreateProductCategory;
