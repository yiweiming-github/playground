from numpy import *
import operator
import matplotlib
import matplotlib.pyplot as plt

#read file and convert it to matrix,
#the file columns are splitted by tab,
#and the last column is label
def prepareMatrixForKnn(filename, featureCount, deli):
    fr = open(filename)
    arrayOfLines = fr.readlines()
    numberOfLines = len(arrayOfLines)
    returnMat = zeros((numberOfLines, featureCount))
    classLabelVector = []
    index = 0
    for line in arrayOfLines:
        line = line.strip()
        listFromLine = line.split(deli)
        returnMat[index,:] = listFromLine[0:featureCount]
        classLabelVector.append(listFromLine[-1])
        index += 1
    fr.close()
    return returnMat, classLabelVector

def normalizeMat(dataSet):
    minVals = dataSet.min(0)
    maxVals = dataSet.max(0)
    ranges = maxVals - minVals
    normDataSet = zeros(shape(dataSet))
    m = dataSet.shape[0]
    normDataSet = dataSet - tile(minVals, (m, 1))
    normDataSet = normDataSet / tile(ranges, (m, 1))
    return normDataSet

def knnClassify(inX, dataSet, labels, k):
    dataSetSize = dataSet.shape[0]
    diffMat = tile(inX, (dataSetSize, 1)) - dataSet
    sqDiffMat = diffMat**2
    sqDistances = sqDiffMat.sum(axis=1)
    distance = sqDistances**0.5
    sortedDistIndicies = distance.argsort()
    classCount = {}
    for i in range(k):
        voteLabel = labels[sortedDistIndicies[i]]
        classCount[voteLabel] = classCount.get(voteLabel, 0) + 1
    sortedClassCount = sorted(classCount.iteritems(), key=operator.itemgetter(1), reverse = True)
    return sortedClassCount[0][0]

def knnTest(dataSet, labels, k, testRatio):
    m = dataSet.shape[0]
    numTestVecs = int(m*testRatio)
    errorCount = 0.0
    for i in range(numTestVecs):
        classifierResult = knnClassify(dataSet[i,:], dataSet[numTestVecs:m,:], labels[numTestVecs:m], k)
        print "test result is %s, real answer is %s" % (classifierResult, labels[i])
        if(classifierResult != labels[i]):
            errorCount += 1.0
            
    print "error rate is: %f" % (errorCount / float(numTestVecs))
    
def knnTestReverse(dataSet, labels, k, testRatio):
    m = dataSet.shape[0]
    numTestVecs = int(m*testRatio)
    errorCount = 0.0
    for i in range(m - numTestVecs, m):
        classifierResult = knnClassify(dataSet[i,:], dataSet[0:i,:], labels[0:i], k)
        print "test result is %s, real answer is %s" % (classifierResult, labels[i])
        if(classifierResult != labels[i]):
            errorCount += 1.0
    print "error rate is: %f" % (errorCount / float(numTestVecs))

#mat, labels = prepareMatrixForKnn("E:\\Doc\\MachineLearning\\MachineLearninginAction\\MLiA_SourceCode\\machinelearninginaction\\Ch02\\datingTestSet.txt", 3, '\t')
mat, labels = prepareMatrixForKnn("E:\\Doc\\Strategy\\399006_knn.csv", 11, ',')
#normalMat = normalizeMat(mat)
normalMat = mat

knnTestReverse(normalMat, labels, 11, 0.025)

k = 11
print "March:" 
print knnClassify((2.913231,0.96065145917398,0.914777033265711,27.6671779675742,45.0258813409728,-7.05022877922283,2069.95032970711,2137.48691073631,-67.5365810292046,0.899638118175003,5.91486), normalMat, labels, k)
print knnClassify((4.273305,1.0205658585382,0.962869605350712,32.8384409395474,40.9634012071643,16.5885204043136,2061.89704821371,2128.60669512621,-66.7096469125086,1.13316099656905,5.056542), normalMat, labels, k)
print knnClassify((-0.521807,1.01833801554898,0.966323062836353,35.4268022553501,39.1178682232262,28.0446703195977,2053.46304079621,2119.60442141316,-66.1413806169489,1.1521619539035,2.81641), normalMat, labels, k)
print knnClassify((-4.984116,0.978292228843436,0.931796912901533,28.9889262973281,35.7415542479268,15.4836703961306,2030.93657298141,2103.85898278996,-72.9224098085538,0.992377224420332,5.395561), normalMat, labels, k)
print knnClassify((2.427373,0.994571063950081,0.968240324286047,28.9378499068507,33.4736528009015,19.8662441187491,2018.99740790735,2092.70883591663,-73.711428009286,0.761614433672725,3.190492), normalMat, labels, k)
print knnClassify((2.501316,1.01251218236346,1.00319654468336,34.2919185752841,33.746408059029,35.3829396077942,2016.41180669083,2086.00381103392,-69.5920043430874,1.01916111901717,6.990158), normalMat, labels, k)
print knnClassify((-1.567183,1.00138326965232,0.999135369326067,42.2922161476867,36.5950107552482,53.6866269325636,2009.39660566147,2077.47115836474,-68.074552703267,0.853556972488278,3.429293), normalMat, labels, k)
print knnClassify((-1.717312,0.991248517836371,0.98698956076965,42.550933904147,38.5803184715478,50.4921647693453,1998.25374325202,2067.06351700439,-68.8097737523733,0.764814912101677,2.853543), normalMat, labels, k)
print knnClassify((-0.108314,0.987362543265715,0.989958536625299,42.4088324207191,39.8564897879382,47.5135176862808,1988.50239813632,2057.27140463369,-68.7690064973726,0.643579345480834,2.306543), normalMat, labels, k)
print knnClassify((4.561443,1.02509837382967,1.02759750104912,54.7999459885632,44.8376418548132,74.7245542560632,1993.82941380766,2054.74226354972,-60.9128497420602,1.44078728252107,4.465003), normalMat, labels, k)
print knnClassify((-1.29542,1.01235980130794,1.01110165229338,58.6459138309148,49.4403991801804,77.0569431323836,1994.30488860648,2050.4591329164,-56.1542443099258,0.974470787132863,1.939867), normalMat, labels, k)
print knnClassify((-0.970595,1.00185076656956,1.00332335914628,59.494657304527,52.7918185549626,72.9003348036559,1991.7253672824,2045.05756751519,-53.3322002327845,1.02055651684941,3.909421), normalMat, labels, k)
print knnClassify((5.550083,1.04158946496564,1.05471606148005,72.7215514837903,59.4350628645718,99.2945287222272,2006.42808000819,2048.18611806962,-41.7580380614309,1.85438127222744,5.208092), normalMat, labels, k)
print knnClassify((4.339448,1.06105586329745,1.08562801422447,80.142655938667,66.3375938892702,107.75278003746,2032.80376000693,2057.79233154594,-24.9885715390153,1.9773543825189,4.420846), normalMat, labels, k)
print knnClassify((2.257435,1.06387093106632,1.09519305126109,86.0237334656675,72.899640414736,112.271919567531,2062.68533539048,2070.32875143143,-7.64341604095171,1.62815169398835,1.96008), normalMat, labels, k)
print knnClassify((-0.239422,1.0390100039873,1.0809026465525,88.1181629695942,77.9724812663554,108.409526376072,2087.1494376381,2081.54158465873,5.60785297936536,1.11240227202985,2.615631), normalMat, labels, k)
print knnClassify((1.683574,1.02939258662509,1.08389765895328,92.0787753130628,82.6745792819245,110.887167375339,2113.60429338608,2094.69450431364,18.9097890724415,0.988787376043399,2.432099), normalMat, labels, k)
print knnClassify((-1.74122,0.999402817455379,1.05076720817869,88.5952042140398,84.6481209259629,96.4893707901934,2129.93747901899,2103.95935584596,25.978123173029,1.04377577252228,2.659194), normalMat, labels, k)
print knnClassify((-0.165963,0.994322530994648,1.03524232676259,85.8848285745808,85.0603568088356,87.5337721060713,2143.19109763146,2112.26503319071,30.9260644407482,0.680354281903621,2.18365), normalMat, labels, k)
print knnClassify((-0.740089,0.989391840792332,1.01917462462593,82.3505758502159,84.1570964892957,78.7375345720564,2151.88246722662,2118.74058628769,33.1418809389243,0.806818778204299,3.013466), normalMat, labels, k)
print knnClassify((-2.142898,0.974250929760903,0.990194783185989,73.9110531532962,80.7417487106292,60.2496620386302,2151.98485688406,2121.24483915527,30.7400177287896,0.790256730283601,2.911053), normalMat, labels, k)
print knnClassify((4.470748,1.01875921003705,1.02171538714187,77.0844821724894,79.5226598645826,72.2081267883031,2166.8768789019,2130.69211032895,36.1847685729426,1.05779117316678,3.532651), normalMat, labels, k)
print knnClassify((-0.466786,1.012305493032,1.01001720600319,75.7403997986361,78.2619065092671,70.6973863773742,2177.86289753237,2138.66202808236,39.2008694500091,1.12368745571401,1.76371), normalMat, labels, k)
print knnClassify((-1.470634,0.998386008963056,0.993930198294431,67.3835223042091,74.6357784409145,52.8790100307985,2182.09460560432,2143.60328526145,38.4913203428678,0.98322418244082,2.48659), normalMat, labels, k)
print knnClassify((3.362294,1.02454886536085,1.02492462632394,77.0288161868251,75.4334576895513,80.2195331813726,2197.08312781904,2153.6711900569,43.41193776214,1.2025564614901,3.616764), normalMat, labels, k)
print knnClassify((0.768627,1.01918590994639,1.02931575160173,83.0879367543159,77.9849507111395,93.2939088406687,2212.46126200072,2164.29117597861,48.1700860221154,1.21017166984625,1.487462), normalMat, labels, k)
print knnClassify((-2.105187,0.997738909742743,1.00811754015956,76.5342172354219,77.5013728859003,74.599905934465,2218.03399092369,2170.54249627649,47.4914946472004,1.08399384743883,2.957936), normalMat, labels, k)
print knnClassify((-0.833821,0.990153409940617,0.999256316203811,68.8008404639309,74.6011954119105,57.2001305679716,2219.86476155081,2174.94186692268,44.9228946281391,0.906926311179067,2.026563), normalMat, labels, k)
print knnClassify((2.386124,1.00682578956569,1.02003472011093,73.1923139076681,74.1315682438297,71.3138052353449,2229.59987515838,2182.95676566914,46.6431094892378,1.0369051228865,2.078223), normalMat, labels, k)
print knnClassify((-0.837749,0.999758187405291,1.00859060082173,70.0572421811267,72.773459556262,64.6248074308559,2234.89466359555,2188.96115339736,45.9335101981983,0.857195566852956,2.325873), normalMat, labels, k)
print knnClassify((1.325035,1.0132761273352,1.01555457306237,72.4689909125233,72.6719700083491,72.0630327208717,2243.99009996547,2196.74291981237,47.2471801531024,1.17865395164518,2.276176), normalMat, labels, k)
print knnClassify((1.324272,1.01987345533352,1.02557040772574,80.457062908503,75.2670009750671,90.8371867753749,2256.35993074001,2206.19855538182,50.1613753581914,0.924607883775096,1.561062), normalMat, labels, k)
print knnClassify((-0.633197,1.00637337210433,1.015876640673,81.9699395591541,77.5013138364294,90.9071910046033,2264.56240293386,2213.86355127946,50.6988516543929,0.773046782379972,1.318064), normalMat, labels, k)
print knnClassify((-1.908623,0.988677564612914,0.993854803400735,72.1596275532446,75.7207517420345,65.0373791756649,2264.72095632865,2217.6953622958,47.0255940328479,0.751200442220337,1.692056), normalMat, labels, k)
print knnClassify((0.305748,0.990964921415102,0.997199703576495,67.3194512086214,72.9203182308968,56.1177171640705,2265.92080920116,2221.75644657018,44.1643626309783,0.726589392113996,1.565021), normalMat, labels, k)
print knnClassify((-5.600875,0.947759569272932,0.947660343715441,48.9649777019943,64.9352047212626,17.0245236634576,2247.35437701637,2216.08848756499,31.2658894513829,1.19089528281845,7.246581), normalMat, labels, k)
print knnClassify((-1.635109,0.950250333462725,0.937904301281159,33.1205612776026,54.3303235733759,-9.29896331394411,2226.24785747539,2208.24208107869,18.0057763966979,0.7120314339133,2.484106), normalMat, labels, k)
print knnClassify((1.268244,0.97751069078842,0.953742001694034,28.4895528583152,45.7167333350223,-5.96480809509917,2212.5057255561,2202.95926025805,9.54646529805132,0.673290137850221,2.178695), normalMat, labels, k)


# fig = plt.figure()
# ax = fig.add_subplot(111)
# ax.scatter(mat[:,1], mat[:,2], c=random.rand(len(set(labels)))**15.0)
# plt.show()