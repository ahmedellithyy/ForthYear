I = imread('I.bmp');
figure, imshow(I), title("Orig");
M = LinearFilter(I, Sobel("H"), "absolute");
figure, imshow(M), title("H");

M = LinearFilter(I, Sobel("V"), "absolute");
figure, imshow(M), title("V");


%  M = LinearFilter(I,Gauss2(4),"none");
%  figure, imshow(I), title("Original Figure");
%  figure, imshow(M), title("Figure_with_guassinan_filter");
%  M = LinearFilter(I,  MeanFilter(5, 5), "none");
% figure, imshow(M), title("WithMeanFilter");
% M = LinearFilter(I,Gauss2(0.3),"none");
% figure, imshow(M), title("WithGuassin2");
% 
% M = EdgeMagnit(I);
% figure, imshow(M), title("Edge_detection");
