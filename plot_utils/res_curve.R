library(ggplot2)
library(reshape2)
library(grid)
library(scales)

# set workspace
dirnames <- c('wiki-Vote', 'ca-AstroPh', 'com-dblp', 'com-lj')
discounts <- c('0.7', '0.85', '1')

# input
dirselect <- readline("Enter 1..4 to choose dataset: ")
disselect <- readline("Enter 1..3 to choose discount: ")
dirname <- dirnames[as.integer(dirselect)]
discount <- discounts[as.integer(disselect)]

# main procedure
loc <- paste('~/../Desktop/results1/', dirname, sep='')

fres <- paste(loc, '/Alpha=', discount, '/curve_c.txt', sep = '')
dres <- read.csv(fres, sep=' ', stringsAsFactors=F, strip.white=TRUE)

# dres$M <- factor(dres$M, levels = c('C', 'S'))
ggplot(dres, aes(x=C, y=S)) +
  # scale_fill_manual(values=c("#CC6666", "#9999CC", "#66CC99")) +
  # scale_fill_manual(values=c('#6E548D', '#DB843D', '#C0504D')) + 
  theme_bw() +
  scale_y_continuous(limits=c(5000, 7000))+
  scale_x_continuous(breaks=round(seq(0.05,1,by=0.05),1)) + 
#   scale_y_log10(breaks=trans_breaks("log10", function(x) 10^x),
#                 labels=trans_format("log10", math_format(10^.x)),
#                 limits=c(1e3, 1e5)) + 
  geom_point(size=3) +
  geom_line(size=1) +
  xlab("Discount") +
  ylab("Influence Spread") +
#   theme(
#     legend.position = c(0.1, 0.8), # c(0,0) bottom left, c(1,1) top-right.
#     legend.title=element_blank(),
#     # legend.key.width=unit(0.6, "cm"),
#     legend.text=element_text(size=9)
#   ) +
  ggtitle(bquote(.(dirname) ~ 'with' ~ alpha ~ '=' ~ .(discount)))
# paste("wiki-Vote with ", alpha, "=1"))
figloc <- paste('~/../Desktop/results1/curve/curve_', 
                dirname, '_', discount, '.eps', sep='')
ggsave(file=figloc)
