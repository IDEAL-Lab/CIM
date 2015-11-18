library(ggplot2)
library(reshape2)
library(grid)

func1 <- function(x) {x}
func2 <- function(x) {x^2}
func3 <- function(x) {2*x-x^2}

lb1 <- data.frame(x = 1, y = 1, label = as.character(expression(cu)))

ggplot(data.frame(x=c(0,1)), aes(x)) +
  theme_bw() +
  stat_function(fun=func1, geom="line", aes(colour="f1"), size=1.1) +
  geom_text(aes(0.5, 0.58, label="c[u]"), parse=TRUE, size=7.5) +
  stat_function(fun=func2, geom="line", aes(colour="f2"), size=1) +
  geom_text(aes(0.3, 0.75, label="2*c[u]-c[u]^2"), parse=TRUE, size=7.5) +
  stat_function(fun=func3, geom="line", aes(colour="f3"), size=1) +
  geom_text(aes(0.7, 0.4, label="c[u]^2"), parse=TRUE, size=7.5) +
  xlab(expression(c[u])) +
  ylab(expression(p[u](c[u]))) +
  theme(
    legend.position="none",
    legend.text=element_text(size=14),
    axis.text=element_text(size=15),
    axis.title=element_text(size=18),
    plot.title=element_text(size=18)
  ) +
  expand_limits(x = 0, y = 0) +
  scale_x_continuous(expand = c(0, 0)) + 
  scale_y_continuous(expand = c(0, 0))
  # scale_colour_manual("Function", value=c("blue","red"), breaks=c("square","exp"))