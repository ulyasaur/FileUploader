import { useState } from "react";
import FileService from "./app/api/files";
import "./index.css";

function App() {
  const [email, setEmail] = useState('');
  const [file, setFile] = useState(null);

  const handleEmailChange = (e) => {
    setEmail(e.target.value);
  };

  const handleFileChange = (e) => {
    setFile(e.target.files[0]);
  };

  const isEmailValid = (email) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  };

  const isFileValid = (file) => {
    const fileExtension = file.name.split(".").pop();
    return fileExtension === "docx";
  }

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!isEmailValid(email)) {
      alert('Please enter a valid email address');
      return;
    }

    if (!file) {
      alert('Please upload a file first');
      return;
    }

    if (!isFileValid(file)) {
      alert('Please upload a docx file');
      return;
    }

    const request = {
      email,
      document: file,
    };

    await FileService.uploadFile(request);
  };

  return (
    <form className="file-form" onSubmit={handleSubmit}>
      <label className="input-label">Email Address:</label>
      <input
        className="form-input"
        type="email"
        value={email}
        autoComplete={true}
        onChange={handleEmailChange}
        required
      />

      <label className="input-label">Upload File:</label>
      <input
        className="form-input"
        type="file"
        onChange={handleFileChange}
        accept=".docx"
        required
      />

      <button className="form-button" type="submit">Submit</button>
    </form>
  );
}

export default App;
