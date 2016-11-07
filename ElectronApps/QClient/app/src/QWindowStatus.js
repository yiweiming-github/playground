import fs from "fs";

export default class QWindowStatus {
    constructor(filePath) {
        this.statusData = JSON.parse(fs.readFileSync(filePath));
    }

    data() {
        return this.statusData;
    }	
}