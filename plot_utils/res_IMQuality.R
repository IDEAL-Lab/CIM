library(ggplot2)
library(reshape2)
library(grid)

# set workspace
dirnames <- c('wiki-Vote', 'ca-AstroPh', 'com-dblp', 'com-lj')
discounts <- c('0.7', '0.85', '1')

# input
dirselect <- readline("Enter 1..4 to choose dataset: ")
disselect <- readline("Enter 1..3 to choose discount: ")
dirname <- dirnames[as.integer(dirselect)]
discount <- discounts[as.integer(disselect)]

# main procedure
loc <- paste('~/Desktop/results/evaluation/', dirname, sep='')

fres <- paste(loc, '/Alpha=', discount, '/IMQuality.txt', sep = '')
dres <- read.csv(fres, sep=' ', stringsAsFactors=F, strip.white=TRUE)

idseq <- c(seq(10, 50, 10))
topline <- c(0.63, 0.63, 0.63, 0.63)

ggplot(dres, aes(x=B, y=AccuracyLB)) +
  theme_bw() +
  scale_y_continuous(labels=percent, limits=c(0.5,0.65)) + 
  geom_point() +
  geom_line() +
  geom_hline(aes(yintercept=0.63), color='#009E73', size=1) +
  geom_text(aes(10, 0.63, label="Approximation Upper Bound (63%)", hjust=-0.5, vjust=-1)) + 
  xlab("Budget") +
  ylab("Approximation Lower Bound") +
  ggtitle(bquote(.(dirname) ~ 'with' ~ alpha ~ '=' ~ .(discount)))
# paste("wiki-Vote with ", alpha, "=1"))
figloc <- paste('~/Desktop/results/imquality/', 
                dirname, '_', discount, '.eps', sep='')
ggsave(file=figloc)
