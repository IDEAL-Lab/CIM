library(ggplot2)
library(reshape2)
library(grid)

# function
# Multiple plot function
#
# ggplot objects can be passed in ..., or to plotlist (as a list of ggplot objects)
# - cols:   Number of columns in layout
# - layout: A matrix specifying the layout. If present, 'cols' is ignored.
#
# If the layout is something like matrix(c(1,2,3,3), nrow=2, byrow=TRUE),
# then plot 1 will go in the upper left, 2 will go in the upper right, and
# 3 will go all the way across the bottom.
#
multiplot <- function(..., plotlist=NULL, file, cols=1, layout=NULL) {
  library(grid)
  
  # Make a list from the ... arguments and plotlist
  plots <- c(list(...), plotlist)
  
  numPlots = length(plots)
  
  # If layout is NULL, then use 'cols' to determine layout
  if (is.null(layout)) {
    # Make the panel
    # ncol: Number of columns of plots
    # nrow: Number of rows needed, calculated from # of cols
    layout <- matrix(seq(1, cols * ceiling(numPlots/cols)),
                     ncol = cols, nrow = ceiling(numPlots/cols))
  }
  
  if (numPlots==1) {
    print(plots[[1]])
    
  } else {
    # Set up the page
    grid.newpage()
    pushViewport(viewport(layout = grid.layout(nrow(layout), ncol(layout))))
    
    # Make each plot, in the correct location
    for (i in 1:numPlots) {
      # Get the i,j matrix positions of the regions that contain this subplot
      matchidx <- as.data.frame(which(layout == i, arr.ind = TRUE))
      
      print(plots[[i]], vp = viewport(layout.pos.row = matchidx$row,
                                      layout.pos.col = matchidx$col))
    }
  }
}

# main procedure
loc <- '~/Desktop/results/'

fres <- paste(loc, 'MC_RES.txt', sep = '')
dres <- read.csv(fres, sep=' ', stringsAsFactors=F, strip.white=TRUE)

p1 <- ggplot(dres, aes(x=nodes)) +
  theme_bw() +
  scale_y_continuous(limits = c(100000, 132000)) +
  scale_x_continuous(breaks=seq(0,200000,40000)) +
  scale_color_manual(values=c(b1="#56B4E9")) +
  geom_point(aes(y=influ, color="b1")) +
  geom_line(aes(y=influ, color="b1")) +
  xlab("Number of Simulations") +
  ylab("Influence Spread Value") +
  theme(
    legend.position="none",
    axis.text=element_text(size=12),
    axis.title=element_text(size=18),
    plot.title=element_text(size=18)
  )

p2 <- ggplot(dres, aes(x=nodes)) +
  theme_bw() +
  scale_y_continuous(limits = c(6500, 8000)) +
  scale_x_continuous(breaks=seq(0,200000,40000)) +
  geom_point(aes(y=std, colour="std")) +
  geom_line(aes(y=std, colour="std")) +
  xlab("Number of Simulations") +
  ylab("Standard Deviation Value") +
  theme(
    legend.position="none",
    axis.text=element_text(size=12),
    axis.title=element_text(size=18),
    plot.title=element_text(size=18)
  )

postscript('~/Desktop/results/MC_RES.eps')
multiplot(p1, p2, cols=2)
dev.off()
# figloc <- paste('~/Desktop/results/', 'MC_RES', '.eps', sep='')
# ggsave(file=figloc)

