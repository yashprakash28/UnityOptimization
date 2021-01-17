import uvicorn
from fastapi import FastAPI, UploadFile, File, Form
import cv2
import numpy as np
from PIL import Image
from io import BytesIO


app = FastAPI()

modelFile = "res10_300x300_ssd_iter_140000_fp16.caffemodel"
configFile = "deploy.prototxt"
net = cv2.dnn.readNetFromCaffe(configFile, modelFile)


def give_coordinates(img):
    gray = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)

    (h, w) = img.shape[:2]
    top_corner = []
    bottom_corner = []
    out = {}

    blob = cv2.dnn.blobFromImage(img, 1.0, (300, 300), [104, 117, 123], True, False)

    net.setInput(blob)
    detections = net.forward()

    for i in range(detections.shape[2]):
        confidence = detections[0, 0, i, 2]
        if confidence > 0.4:
            box = detections[0, 0, i, 3:7] * np.array([w, h, w, h])
            (x1, y1, x2, y2) = box.astype("int")
            top = int(x1), int(y1)
            top_corner.append(top)
            bottom = int(x2), int(y2)
            bottom_corner.append(bottom)
            out['bottom_right_corner'] = bottom_corner
            out['top_left_corner'] = top_corner

    return out



@app.get("/")
def home():
    return "This API is working just fine."


def load_image_into_numpy_array(data):
    return np.array(Image.open(BytesIO(data)))

@app.post("/getcoordinates")
async def get_image(file: UploadFile = File(...)):
    image = load_image_into_numpy_array(await file.read())
    output = give_coordinates(image)

    return output



if __name__ == "__main__":
    uvicorn.run(app, host="127.0.0.1", port=8000, debug=True)
