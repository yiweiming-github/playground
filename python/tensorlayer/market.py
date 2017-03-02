#! /usr/bin/python

import math
import numpy as np
import matplotlib as mpl
import matplotlib.pyplot as plt

class MarketStatistics:
    def __init__(self, folder, trainingWindow = 60):
        self.trainingWindow = trainingWindow
        self.filePath = folder
        self.pv = []

    def plotPVStats(self):
        indexData = np.loadtxt(self.filePath + '\\index.txt')
        for stock in indexData:
            prices = np.loadtxt(self.filePath + '\\' + str(int(stock[0])) + '_prices.txt')
            sampleCount = len(prices) - self.trainingWindow + 1
            #print('samplecCount: %d  ' % (sampleCount))
            for i in range(0, sampleCount):
                if prices[i + self.trainingWindow - 1][2] > 0 and prices[i + 1][3] > 0:
                    self.pv.append(prices[i + self.trainingWindow - 1][2]/prices[i + 1][3])
            print('Finished adding %d' % (int(stock[0])))       
            
        print('********    Statistics of PV    ********')
        print('mean: %f    std: %f' % (np.mean(self.pv), np.std(self.pv)))
        plt.hist(self.pv, np.arange(0, 3.0, 0.005))
        plt.show()
    
    def getPV(self):
        return self.pv


class MarketEnv:
    def __init__(self):        
        self.trainingWindow = 60
        self.slippage = 0
        self.pv = 1.0
        self.shape = (-1, 50, 20, 1)
        self.position = 0.0
        self.cashValue = 1.0

    def initialize(self, filePath):
        # read data from filePath
        self.indexData = np.loadtxt(filePath + '\\index.txt')
        self.folder = filePath

    def getPV(self):
        return self.pv
    
    """    
    action: 1 - buy to 20%, 2 - buy to 40%, 3 - buy to 60%, 4 - buy to 80%, 5 -  buy to 100%
            6 - sell to 80%, 7 - sell to 60%, 8 - sell to 40%, 9 - sell to 20%, 10 - sell to 0%
            11 - do nothing
    """
    def nextTradingCycle(self, action, useCNN = False):        
        reward = 0
        executionPrice = self.prices[self.cursor + 1][3] #next open price
        closePrice = self.prices[self.cursor + 1][2]

        #print ('action: %d, executionPrice: %f, self.position: %f, self.averagePrice: %f, self.cashValue: %f, self.pv: %f' % (action, executionPrice, self.position, self.averagePrice, self.cashValue, self.pv))

        if action == 1:   # buy to 20%        
            self.updatePriceAndPositionForBuy(0.2, executionPrice)
        elif action == 2: # buy to 40%
            self.updatePriceAndPositionForBuy(0.4, executionPrice)                    
        elif action == 3: # buy to 60%
            self.updatePriceAndPositionForBuy(0.6, executionPrice)
        elif action == 4: # buy to 80%
            self.updatePriceAndPositionForBuy(0.8, executionPrice)
        elif action == 5: # buy to 100%
            self.updatePriceAndPositionForBuy(1.0, executionPrice)
        elif action == 6: # sell to 80%
            reward = self.updatePriceAndPositionForSell(0.8, executionPrice)
        elif action == 7: # sell to 60%
            reward = self.updatePriceAndPositionForSell(0.6, executionPrice)
        elif action == 8: # sell to 40%
            reward = self.updatePriceAndPositionForSell(0.4, executionPrice)
        elif action == 9: # sell to 20%
            reward = self.updatePriceAndPositionForSell(0.2, executionPrice)
        elif action == 10: # sell to 0%
            reward = self.updatePriceAndPositionForSell(0.0, executionPrice)            
        else:
            pass
        
        reward += self.updatePositionValue(closePrice)

        #print ('************************************* self.position: %f, self.averagePrice: %f, self.cashValue: %f, self.pv: %f' % (self.position, self.averagePrice, self.cashValue,  self.pv))
        #print ('')

        self.cursor += 1
        done = (self.cursor >= self.trainingWindow - 1)
        nextCycle = self.trainingCycles[self.cursor].reshape(self.shape) if useCNN else self.trainingCycles[self.cursor]
        return nextCycle, reward, done

    def updatePriceAndPositionForBuy(self, targetPosition, executionPrice):
        positionRatio = 1.0 - self.cashValue / self.pv
        if positionRatio < targetPosition:
            tradePrice = executionPrice * (1 + self.slippage)
            if tradePrice > 0.0:
                if self.position == 0.0:
                    positionToBuy = targetPosition * self.pv / tradePrice
                    self.averagePrice = tradePrice
                    self.position += positionToBuy                
                    self.cashValue -= positionToBuy * tradePrice
                else:
                    positionToBuy = (self.pv * targetPosition - (self.pv - self.cashValue)) / tradePrice
                    self.averagePrice = self.pv * targetPosition / (self.position + positionToBuy)
                    self.position += positionToBuy                
                    self.cashValue -= positionToBuy * tradePrice
            
    
    def updatePriceAndPositionForSell(self, targetPosition, executionPrice):
        reward = 0.0
        positionRatio = 1.0 - self.cashValue / self.pv
        if positionRatio > targetPosition:
            tradePrice = executionPrice
            if tradePrice > 0.0:
                positionToSell = ((self.pv - self.cashValue) - self.pv * targetPosition) / tradePrice
                self.cashValue += positionToSell * tradePrice

                reward = (tradePrice - self.averagePrice) * positionToSell / self.pv                
                if targetPosition < 0.2:
                    self.averagePrice = 0.0
                    self.position = 0.0
                else:
                    self.averagePrice = self.pv * targetPosition / (self.position - positionToSell)
                    self.position -= positionToSell
        return reward
    
    def updatePositionValue(self, closePrice):
        reward = (closePrice - self.averagePrice) * self.position / self.pv
        self.pv = self.cashValue + self.position * closePrice
        return reward

    """
    action: 1 - buy, 2 - sell, 3 - nothing
    """
    def nextTradingCycleWithoutPMS(self, action, useCNN = False):        
        reward = 0
        executionPrice = self.prices[self.cursor + 1][3] #next open price

        if action == 1: 
            # buy
            if self.averagePrice == 0:
                self.averagePrice = executionPrice* (1 + self.slippage)
        elif action == 2:
            # sell
            if self.averagePrice > 0:                
                reward = executionPrice / self.averagePrice - 1.0
                self.pv = self.pv * (executionPrice / self.averagePrice)
                self.averagePrice = 0
        else:
            pass
        
        self.cursor += 1
        done = (self.cursor >= self.trainingWindow - 1)
        nextCycle = self.trainingCycles[self.cursor].reshape(self.shape) if useCNN else self.trainingCycles[self.cursor]
        return nextCycle, reward, done

    """
    randomly pick up a trading period and return the first trading cycle
    """
    def reset(self, useCNN = False):
        while True:
            self.categary = math.ceil(np.random.rand() * len(self.indexData))
            situationSeries = np.loadtxt(self.folder + '\\' + str(self.categary) + '_charts.txt')
            priceSeries = np.loadtxt(self.folder + '\\' + str(self.categary) + '_prices.txt')
            startPosition = math.ceil(np.random.rand() * (len(situationSeries) - self.trainingWindow - 1))
            #print("trying categary %d and start with position %d." % (self.categary, startPosition))
            if startPosition > 0:
                self.trainingCycles = situationSeries[startPosition : startPosition + self.trainingWindow]
                self.prices = priceSeries[startPosition : startPosition + self.trainingWindow + 1]
                self.cursor = 0
                self.averagePrice = 0.0
                self.cashValue = 1.0
                self.pv = 1.0
                self.position = 0.0
                print("Picked up categary %d and start with position %d." % (self.categary, startPosition))
                
                return self.trainingCycles[self.cursor].reshape(self.shape) if useCNN else self.trainingCycles[self.cursor]


