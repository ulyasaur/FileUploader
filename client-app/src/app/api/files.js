import axios from "./axios";

const baseUrl = "/files"

class FileService {
    async uploadFile(request) {
        const formData = new FormData();
        formData.append("Email", request.email);
        formData.append("Document", request.document);

        await axios.post(baseUrl, formData, {
            headers: { "Content-Type": "multipart/form-data" }
        });
    }
}

export default new FileService();