# Idle-API 

Just small sample api to fool around with kubernetes autoscaling on custom metrics.

## Steps done

- Dockerfile adapted to build docker image use 
  - `docker build -t idle-api:1.0 .`
  - `docker container run -d --name my-idle-api -p 32666:80 idle-api:1.0` to run locally	
- Push to dockerhub
  - `docker tag idle-api:1.0 <dockerhub-user>/idle-api:1.0`
  - `docker push <dockerhub-user>/idle-api:1.0`

- Deployed to kubernetes cluster
  - `kubectl apply -f deployment.yaml`
- Create Service: : To access your application from outside the Kubernetes cluster, you need to create a service. Save the following YAML to a file named service.yaml
  - `kubectl apply -f service.yaml`


## Challenges

### 1. convert string into numerical metric!

```txt
my api has an status endpoint: "localhost:32666/status" which returns strings "idle" or "busy" ...if all pods are busy, a new on shall be created. how do i accomplish that?
```

```txt
  To autoscale your Kubernetes pods based on the status of your API, you can use the Kubernetes Horizontal Pod Autoscaler (HPA) with custom metrics¹. However, Kubernetes does not natively support text-based metrics like "idle" or "busy". You would need to convert these statuses into numerical metrics that Kubernetes can understand.

Here's a general approach you could take:

1. **Create a Metrics Server**: You would need a server to expose your custom metrics to Kubernetes. This could be done using Prometheus¹ or a similar system. This server would query the `/status` endpoint of your API and convert the "idle" or "busy" status into a numerical metric. For example, "idle" could be `0` and "busy" could be `1`.

2. **Configure Custom Metrics API**: Kubernetes retrieves custom metrics through the Custom Metrics API, so you would need to ensure this API is available in your cluster¹. You might need to deploy an adapter like the Prometheus adapter if you're using Prometheus.

3. **Set Up Horizontal Pod Autoscaler**: Once your metric is available in the Custom Metrics API, you can create an HPA that scales based on this metric¹. You would specify your metric in the `metrics` field of the HPA configuration.

Please note that setting up autoscaling with custom metrics can be complex and requires a good understanding of several Kubernetes concepts¹. You might want to consider alternative approaches, such as using a readiness probe that checks the `/status` endpoint of your API and only marks the pod as ready if the status is "idle". This way, if all pods are busy, they would be marked as not ready, and Kubernetes would automatically create new pods if needed.

Source: Conversation with Bing, 29/09/2023
(1) How to autoscale apps on Kubernetes with custom metrics. https://learnk8s.io/autoscaling-apps-kubernetes.
(2) Optimize Pod autoscaling based on metrics | Kubernetes Engine | Google .... https://cloud.google.com/kubernetes-engine/docs/tutorials/autoscaling-metrics.
(3) Configure Kubernetes Autoscaling with Custom Metrics - Bitnami. https://docs.bitnami.com/tutorials/configure-autoscaling-custom-metrics/.
(4) About autoscaling workloads based on metrics | Google Kubernetes Engine .... https://cloud.google.com/kubernetes-engine/docs/concepts/custom-and-external-metrics.
```

### Apply manifests

To deploy your .NET 7 application on Kubernetes using the Dockerfile you provided, you can follow these steps:

1. **Build the Docker Image**: First, you need to build the Docker image from your Dockerfile. Navigate to the directory containing your Dockerfile and run the following command:

```bash
docker build -t myapp:1.0 .
```

This command builds a Docker image using your Dockerfile and tags it as `myapp:1.0`.

2. **Push the Docker Image to a Registry**: Kubernetes needs to pull the Docker image from a registry. If you're using Docker Hub, you can push your image there with the following commands:

```bash
docker tag myapp:1.0 username/myapp:1.0
docker push username/myapp:1.0
```

Replace `username` with your Docker Hub username.

3. **Create a Deployment in Kubernetes**: Now that your image is available in a registry, you can create a deployment in Kubernetes. Save the following YAML to a file named `deployment.yaml`:

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: myapp-deployment
spec:
  replicas: 3
  selector:
    matchLabels:
      app: myapp
  template:
    metadata:
      labels:
        app: myapp
    spec:
      containers:
      - name: myapp
        image: username/myapp:1.0
        ports:
        - containerPort: 80
```

Replace `username` with your Docker Hub username.

4. **Apply the Deployment**: You can create the deployment in Kubernetes with the following command:

```bash
kubectl apply -f deployment.yaml
```

5. **Create a Service**: To access your application from outside the Kubernetes cluster, you need to create a service. Save the following YAML to a file named `service.yaml`:

```yaml
apiVersion: v1
kind: Service
metadata:
  name: myapp-service
spec:
  type: LoadBalancer
  ports:
  - port: 80
  selector:
    app: myapp
```

6. **Apply the Service**: You can create the service in Kubernetes with the following command:

```bash
kubectl apply -f service.yaml
```

Now, your .NET 7 application should be running on Kubernetes¹²³⁴. Please replace `myapp`, `myapp-deployment`, and `myapp-service` with names that are appropriate for your application.

Source: Conversation with Bing, 29/09/2023
(1) Containerize an app with Docker tutorial - .NET | Microsoft Learn. https://learn.microsoft.com/en-us/dotnet/core/docker/build-container.
(2) Build ASP.NET Core applications deployed as Linux containers into an .... https://learn.microsoft.com/en-us/dotnet/architecture/containerized-lifecycle/design-develop-containerized-apps/build-aspnet-core-applications-linux-containers-aks-kubernetes.
(3) Deploying a containerized web application | Kubernetes Engine | Google .... https://cloud.google.com/kubernetes-engine/docs/tutorials/hello-app.
(4) Deploy And Run .NET Core 7 Web API On Azure Kubernetes Service (AKS). https://www.c-sharpcorner.com/article/deploy-and-run-net-core-7-web-api-on-azure-kubernetes-service-aks/.
(5) linux - How to deploy .net core in Kubernetes? - Stack Overflow. https://stackoverflow.com/questions/58483863/how-to-deploy-net-core-in-kubernetes.


### Access the application

To access your API endpoint in the browser, you need to know the IP address and port where your service is running. If you're using Minikube, you can get the IP address with the command `minikube ip`¹³. The port would be the one you specified in your service definition.

If you set up your service as type `LoadBalancer`, you can get the external IP and port with the command `kubectl get svc`¹. This will show a list of your services, and you should see an external IP and port for your service. You can then access your API in the browser at `http://<external-ip>:<port>`¹.

If you set up your service as type `NodePort`, you can access your service at `http://<node-ip>:<node-port>`¹. The node IP would be the IP address of any node in your cluster, and the node port would be the one automatically assigned by Kubernetes when you created the service¹.

If you're running Kubernetes locally using Docker Desktop, and if your service is exposed on a certain port, it should be available at `http://localhost:<port>`⁴.

Please replace `<external-ip>`, `<port>`, `<node-ip>`, and `<node-port>` with the actual values for your setup.

Source: Conversation with Bing, 29/09/2023
(1) How to access Kubernetes NodePort service in browser?. https://stackoverflow.com/questions/48253318/how-to-access-kubernetes-nodeport-service-in-browser.
(2) How to run kubernetes service in web browser? - Stack Overflow. https://stackoverflow.com/questions/59475657/how-to-run-kubernetes-service-in-web-browser.
(3) Access a Kubernetes Service running locally in Docker For Desktop .... https://stackoverflow.com/questions/50178696/access-a-kubernetes-service-running-locally-in-docker-for-desktop.
(4) Access Services Running on Clusters | Kubernetes. https://kubernetes.io/docs/tasks/access-application-cluster/access-cluster-services/.
(5) 4 Simple Ways How to Connect to Kubernetes Services - SiteReq. https://www.sitereq.com/post/4-simple-ways-how-to-connect-to-kubernetes-services.
(6) undefined. http://jenkins-ui.default:8080.
(7) undefined. http://10.47.10.xx:8080.
(8) undefined. http://192.168.99.100:30316.


