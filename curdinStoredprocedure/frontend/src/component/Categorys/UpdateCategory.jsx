import React, { useState } from "react";
import axios from "axios";

const UpdateCategory = ({ category, onUpdateComplete }) => {
    const [categoryName, setCategoryName] = useState(category.categoryName);
    const [description, setDescription] = useState(category.description);
    const [error, setError] = useState("");

    const handleUpdate = async () => {
        try {
            await axios.put(`https://localhost:7217/api/productcategories/${category.categoryId}`, {
                categoryName,
                description,
            });
            onUpdateComplete(); // Notify parent to refresh categories
        } catch (err) {
            setError("Failed to update category.");
        }
    };

    return (
        <div>
            <h3>Edit Category</h3>
            {error && <div className="alert alert-danger">{error}</div>}
            <div className="mb-3">
                <label className="form-label">Category Name</label>
                <input
                    type="text"
                    className="form-control"
                    value={categoryName}
                    onChange={(e) => setCategoryName(e.target.value)}
                />
            </div>
            <div className="mb-3">
                <label className="form-label">Description</label>
                <input
                    type="text"
                    className="form-control"
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                />
            </div>
            <button className="btn btn-primary" onClick={handleUpdate}>
                Save
            </button>
        </div>
    );
};

export default UpdateCategory;
