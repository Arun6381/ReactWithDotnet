import React, { useState } from "react";
import axios from "axios";

const UpdateProductItem = ({ product, onUpdateComplete }) => {
    const [productName, setProductName] = useState(product.productName);
    const [price, setPrice] = useState(product.price); // New state for price
    const [categoryId, setCategoryId] = useState(product.categoryId);
    const [description, setDescription] = useState(product.description);
    const [error, setError] = useState("");

    const handleUpdate = async () => {
        try {
            await axios.put(`https://localhost:7217/api/productitems/${product.product_Id}`, {
                productName,
                price, // Include price in the request body
                categoryId,
                description,
            });
            onUpdateComplete();
        } catch (err) {
            setError("Failed to update product item.");
        }
    };

    return (
        <div>
            <h3>Update Product Item</h3>
            {error && <div className="alert alert-danger">{error}</div>}
            <div className="mb-3">
                <label className="form-label">Product Name</label>
                <input
                    type="text"
                    className="form-control"
                    value={productName}
                    onChange={(e) => setProductName(e.target.value)}
                />
            </div>
            <div className="mb-3">
                <label className="form-label">Price</label> {/* New Price Field */}
                <input
                    type="number"
                    className="form-control"
                    value={price}
                    onChange={(e) => setPrice(e.target.value)}
                />
            </div>
            <div className="mb-3">
                <label className="form-label">Category ID</label>
                <input
                    type="text"
                    className="form-control"
                    value={categoryId}
                    onChange={(e) => setCategoryId(e.target.value)}
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
                Update
            </button>
        </div>
    );
};

export default UpdateProductItem;
