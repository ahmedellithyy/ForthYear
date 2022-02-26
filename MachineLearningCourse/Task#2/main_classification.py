import pickle
import pandas as pd
import numpy as np
import math
import time
import matplotlib.pyplot as plt
import seaborn as sns
from sklearn import linear_model
from sklearn import metrics
from sklearn.preprocessing import LabelEncoder
from sklearn.model_selection import train_test_split
from sklearn import svm, datasets
from sklearn.linear_model import LogisticRegression
from sklearn.svm import LinearSVC
from sklearn.metrics import classification_report, confusion_matrix
from sklearn.svm import SVC
from sklearn.multiclass import OneVsRestClassifier, OneVsOneClassifier

def preProcess(data):
    #drop data that has no grade
    idx = 0
    nf = []
    for i in data['ProductGrade']:
        if not (i == 'A' or i == 'B' or i == 'C' or i == 'D'):
            nf.append(idx)
        idx += 1
    data = data.drop(nf, axis=0)

    data = data.dropna(subset=['uniq_id', 'ProductGrade', 'product_name', 'price'])
    data['number_available_in_stock'] = data['number_available_in_stock'].str.replace('new', '')
    data['price'] = pd.to_numeric(data['price'], downcast='float', errors='coerce')
    data['number_available_in_stock'] = pd.to_numeric(data['number_available_in_stock'], downcast='float', errors='coerce')
    data['number_of_reviews'] = pd.to_numeric(data['number_of_reviews'], downcast='float', errors='coerce')
    data['number_of_answered_questions'] = pd.to_numeric(data['number_of_answered_questions'], downcast='float', errors='coerce')
    #print(data['price'])
    for i in data:
        random_sample = data[i].dropna().sample(data[i].isnull().sum())
        random_sample.index = data[data[i].isnull()].index
        data.loc[data[i].isnull(), i] = random_sample
    data = data.drop('uniq_id', axis=1)
    data = data.drop('product_name', axis=1)
    data = data.drop('product_information', axis=1)

    #classify categotries into two features: first and second categories
    categories = []
    for i in data['amazon_category_and_sub_category']:
        cur = i.split('>')
        categories.append(cur)
    tmp = pd.DataFrame(categories, columns=['category1', 'category2', 'category3', 'category4', 'category5'])
    data['category1'] = tmp['category1']
    data['category2'] = tmp['category2']
    data = data.drop('amazon_category_and_sub_category', axis=1)

    # classify sellers into: seller names and sellers prices
    sellers = []
    cnt = 0
    for i in data['sellers']:
        cnt += 1
        if cnt % 100 == 0:
            print('Pre-Processing', int((cnt / len(data['sellers'])) * 100), '%')
        cur = i.split('{')
        if len(cur) <= 2:
            continue
        arr = []
        for j in range(2, min(5, len(cur))):
            cur2 = cur[j].split("=>")
            #print(cur2)
            name_price = cur2[1].split(',')
            arr.append(name_price[0])
            arr.append(cur2[2])
        sellers.append(arr)
        for j in sellers:
            for l in range(1, len(j), 2):
                price = ''
                for k in j[l]:
                    if(k >= '0' and k <= '9' or (k == '.')):
                        price += k
                j[l] = price
    tmp = pd.DataFrame(sellers, columns=['seller_name_1', 'seller_price_1', 'seller_name_2', 'seller_price_2', 'seller_name_3', 'seller_price_3'])
    data['seller_name_1'] = tmp['seller_name_1']
    data['seller_name_2'] = tmp['seller_name_2']
    data['seller_name_3'] = tmp['seller_name_3']
    data['seller_price_1'] = tmp['seller_price_1']
    data['seller_price_2'] = tmp['seller_price_2']
    data['seller_price_3'] = tmp['seller_price_3']
    data = data.drop('sellers', axis=1)
    data['seller_price_1'] = pd.to_numeric(data['seller_price_1'], downcast='float', errors='coerce')
    data['seller_price_2'] = pd.to_numeric(data['seller_price_2'], downcast='float', errors='coerce')
    data['seller_price_3'] = pd.to_numeric(data['seller_price_3'], downcast='float', errors='coerce')

    for i in data:
        random_sample = data[i].dropna().sample(data[i].isnull().sum())
        random_sample.index = data[data[i].isnull()].index
        data.loc[data[i].isnull(), i] = random_sample
    print(data.isna().sum())
    return data

def Feature_Encoder(X, col):
    for c in col:
        lbl = LabelEncoder()
        lbl.fit(list(X[c].values))
        X[c] = lbl.transform(list(X[c].values))
    return X

# plt.rcParams['font.size'] = '32'
# models = ['Ploy Kernel SVM', 'RBF Kernel SVM', 'Logisitic Regression']
# acc = [56.3, 51.5, 67.0]
# trainTime = [1.89, 3.30, 15.99]
# testTime = [0.15, 0.96, 0.06]
# fig = plt.figure(figsize=(20, 20))
# plt.bar(models, testTime, color='blue', width=0.5)
# plt.xlabel('Model')
# plt.ylabel('Testing Time (seconds)')
# plt.title('Testing Time of models')
# plt.savefig('test.png', dpi=400)
# plt.show()
# exit(0)


data = pd.read_csv('AmazonProductClassification.csv')
print(data.shape)
data = preProcess(data)
print(data)
print(data.shape)
cols =('manufacturer', 'category1', 'category2', 'seller_name_1', 'seller_name_2', 'seller_name_3')
data = Feature_Encoder(data, cols)
amazon_data = data.iloc[:, :]
X = data.drop('ProductGrade', axis=1)
Y = data['ProductGrade']
X_train, X_test, Y_train, Y_test = train_test_split(X, Y, test_size=0.2, shuffle=True)

#Model#1: poly Kerenl SVM
print('Training Model #1 starts')
startTrain = time.time()
model1 = SVC(kernel='poly', degree=5, C=1.0, max_iter=10000)
model1.fit(X_train, Y_train)
endTrain = time.time()
print('Training Model #1 ended')
print('Testing Model #1 starts')
startTest = time.time()
accuracy = model1.score(X_test, Y_test)
endTest = time.time()
print('Testing Model #1 ended')
print('Model #1 Accuracy: ', accuracy)
print('Model #1 Training Time: ', endTrain - startTrain, ' seconds')
print('Model #1 Testing Time: ', endTest - startTest, ' seconds')

#Model#2: rbf Kernel SVM
print('Training Model #2 starts')
startTrain = time.time()
model2 = SVC(kernel='rbf', gamma=0.8, C=1.0, max_iter=10000)
model2.fit(X_train, Y_train)
endTrain = time.time()
print('Training Model #2 ended')
print('Testing Model #2 starts')
startTest = time.time()
accuracy = model2.score(X_test, Y_test)
endTest = time.time()
print('Testing Model #2 ended')
print('Model #2 Accuracy: ', accuracy)
print('Model #2 Training Time: ', endTrain - startTrain, ' seconds')
print('Model #2 Testing Time: ', endTest - startTest, ' seconds')


#Modle#3: Logistic Regression
print('Training Model #3 starts')
startTrain = time.time()
model3 = LogisticRegression(C=1.0, max_iter=10000).fit(X_train, Y_train)
endTrain = time.time()
startTest = time.time()
accuracy = model3.score(X_test, Y_test)
endTest = time.time()
print('Testing Model #3 ended ended')
print('Accuracy: ', accuracy)
print('Training Time: ', endTrain - startTrain, ' seconds')
print('Testing Time: ', endTest - startTest, ' seconds')



# pickle.dump(model1, open('model1.sav', 'wb'))
# pickle.dump(model2, open('model2.sav', 'wb'))
# pickle.dump(model3, open('model3.sav', 'wb'))
#

#Testing
# data = pd.read_csv('AmazonProductClassification.csv')
# data = preProcess(data)
# cols =('manufacturer', 'category1', 'category2', 'seller_name_1', 'seller_name_2', 'seller_name_3')
# data = Feature_Encoder(data, cols)
# amazon_data = data.iloc[:, :]
# Xtst = data.drop('ProductGrade', axis=1)
# Ytst = data['ProductGrade']
# model1 = pickle.load(open('model1.sav', 'rb'))
# model2 = pickle.load(open('model2.sav', 'rb'))
# model3 = pickle.load(open('model3.sav', 'rb'))
# print('Model #1 Accuracy: ', model1.score(Xtst, Ytst))
# print('Model #2 Accuracy: ', model2.score(Xtst, Ytst))
# print('Model #3 Accuracy: ', model3.score(Xtst, Ytst))