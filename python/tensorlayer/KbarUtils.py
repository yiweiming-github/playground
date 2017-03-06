import matplotlib.pyplot as plt
import matplotlib.finance as mpf

def plotKBarsFromChart(chart, row, col):    
    dataList = []    
    for i in range(0, col):
        (last, high, low, open, close) = (0, 0, 0, 0, 0)        
        for j in range(0, row):
            if chart[j][i] == 1:
                if last == 0:
                    high = row - j
                    close = row - j                
                elif last == 2:
                    close = row - j
                    open = row - j
                else:
                    open = row - j
                    low = row - j
            elif chart[j][i] == 2:
                if last == 0:
                    high = row - j
                else:
                    low = row - j
            elif chart[j][i] == -1:
                if last == 0:
                    high = row - j
                    open = row - j
                    low = row - j
                elif last == -2:
                    open = row - j
                    close = row - j                
                else:
                    low = row - j
                    close = row - j
            elif chart[j][i] == -2:
                if last == 0:
                    high = row - j
                else:
                    low = row - j
            last = chart[j][i]
        bar = (i+1, open, high, low, close)
        dataList.append(bar)
    
    fig, ax = plt.subplots()
    fig.subplots_adjust(bottom=0.2)    
    #ax.xaxis_date()
    ax.autoscale_view()    
    plt.xticks(rotation=45)
    plt.yticks()
    plt.title("K bars")
    plt.xlabel("seq")
    plt.ylabel("price point")
    mpf.candlestick_ohlc(ax,dataList,width=0.5,colorup='r',colordown='green')
    plt.grid()
    plt.show()

def plotKBarsFromPrices(prices, pvList):
    dataList = []
    i = 1
    for price in prices:
        bar = (i, price[3], price[0], price[1], price[2])
        dataList.append(bar)
        i += 1

    fig, ax = plt.subplots()
    fig.subplots_adjust(bottom=0.2)    
    #ax.xaxis_date()
    ax.autoscale_view()    
    plt.xticks(rotation=45)
    plt.yticks()
    plt.title("K bars")
    plt.xlabel("seq")
    plt.ylabel("price point")
    mpf.candlestick_ohlc(ax,dataList,width=0.5,colorup='r',colordown='green')

    if pvList is not None:
        x = range(1, len(pvList) + 1)        
        plt.plot(x, pvList)

    plt.grid()
    plt.show()